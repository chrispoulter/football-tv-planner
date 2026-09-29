#!/bin/sh
set -e

{
  echo "window.__ENV__ ||= {"
  [ -n "${VITE_FEATURE_X}" ] && echo "  VITE_FEATURE_X: \"${VITE_FEATURE_X}\","
  echo "};"
} > /usr/share/nginx/html/env.js