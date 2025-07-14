import 'zx/globals'

console.log("Initialization BidCalculationTool...");

try {
  await $`docker-compose down`;
} catch {
  console.warn('No container to stop (probably first run)');
}

await fs.ensureDir('docs/diagrams');
await $`docker-compose -f docker-compose.parent.yml -f docker-compose.dev.yml up --build -d`;

console.log("Project is ready !");
console.log("To start Frontend and Backend containers: npm run dev");
