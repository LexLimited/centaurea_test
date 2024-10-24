import { defineConfig } from "vite";
import { fileURLToPath, URL } from "url";
import { brotliCompress } from 'zlib';
import { promisify } from 'util';
import react from "@vitejs/plugin-react";
import gzip from 'rollup-plugin-gzip';

export default defineConfig({
    base: "/app",
    plugins: [
        react(),
        gzip({
            customCompression: content => promisify(brotliCompress)(Buffer.from(content)),
            fileName: ".br"
        }),
        gzip()
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    build: {
        rollupOptions: {
            output: {
                manualChunks: {
                    "react-router": ["react", "react-dom", "react-router-dom"],
                    "@fluentui/deps": ["@fluentui/react-theme", "@griffel/react", "@fluentui/react-tabster"],
                    "@fluentui/react-components": ["@fluentui/react-components", "@fluentui/react-components/unstable"]
                }
            }
        }
    },
    server: {
        port: 3000
    }
});
