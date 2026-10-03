import { readFileSync, writeFileSync } from 'node:fs';
import { resolve, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';
import sharp from 'sharp';

const __dirname = dirname(fileURLToPath(import.meta.url));
const root = resolve(__dirname, '..');
const input = resolve(root, 'public/icon.svg');
const svgBuffer = readFileSync(input);

const sizes = [
    { name: 'favicon-16x16.png', size: 16 },
    { name: 'favicon-32x32.png', size: 32 },
    { name: 'favicon-96x96.png', size: 96 },
    { name: 'apple-touch-icon.png', size: 180 },
    { name: 'android-chrome-192x192.png', size: 192 },
    { name: 'android-chrome-512x512.png', size: 512 },
];

for (const { name, size } of sizes) {
    const out = resolve(root, 'public', name);
    await sharp(svgBuffer).resize(size, size).png().toFile(out);
    console.log(`Generated ${name}`);
}

// Build favicon.ico from 16x16 and 32x32 PNGs
const icoSizes = [16, 32];
const pngBuffers = await Promise.all(
    icoSizes.map((s) => sharp(svgBuffer).resize(s, s).png().toBuffer())
);

// Write minimal ICO: ICONDIR + ICONDIRENTRYs + image data
const numImages = pngBuffers.length;
const ICONDIR_SIZE = 6;
const ICONDIRENTRY_SIZE = 16;
const headerSize = ICONDIR_SIZE + ICONDIRENTRY_SIZE * numImages;

let dataOffset = headerSize;
const entries = pngBuffers.map((buf, i) => {
    const size = icoSizes[i];
    const entry = { size, buf, offset: dataOffset };
    dataOffset += buf.length;
    return entry;
});

const totalSize = dataOffset;
const ico = Buffer.alloc(totalSize);

// ICONDIR header
ico.writeUInt16LE(0, 0); // reserved
ico.writeUInt16LE(1, 2); // type: 1 = ICO
ico.writeUInt16LE(numImages, 4);

// ICONDIRENTRY for each image
entries.forEach(({ size, buf, offset }, i) => {
    const base = ICONDIR_SIZE + i * ICONDIRENTRY_SIZE;
    ico.writeUInt8(size === 256 ? 0 : size, base); // width (0 = 256)
    ico.writeUInt8(size === 256 ? 0 : size, base + 1); // height
    ico.writeUInt8(0, base + 2); // color count
    ico.writeUInt8(0, base + 3); // reserved
    ico.writeUInt16LE(1, base + 4); // color planes
    ico.writeUInt16LE(32, base + 6); // bits per pixel
    ico.writeUInt32LE(buf.length, base + 8); // image size
    ico.writeUInt32LE(offset, base + 12); // image offset
});

// Image data
entries.forEach(({ buf, offset }) => {
    buf.copy(ico, offset);
});

const icoPath = resolve(root, 'public/favicon.ico');
writeFileSync(icoPath, ico);
console.log('Generated favicon.ico');
