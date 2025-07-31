import 'zx/globals'

console.log("Initialization BidCalculationTool...");

try {
    await $`docker compose version`;
} catch {
    console.error("Error : `docker compose` command not found.");
    console.error("Make sure Docker is installed with the Compose plugin (version >= 20.10).");
    process.exit(1);
}

try {
  await $`docker compose down`;
} catch {
  console.warn('No container to stop (probably first run)');
}

await fs.ensureDir('docs/diagrams');
await $`docker compose -f docker-compose.parent.yml -f docker-compose.dev.yml up --build -d`;

console.log("Project is ready !");
console.log("To start Frontend and Backend containers: npm run dev");
