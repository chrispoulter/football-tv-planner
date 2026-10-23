interface MetadataProps {
    title: string;
}

export function Metadata({ title }: MetadataProps) {
    return <title>{`${title} | Football TV Planner`}</title>;
}
