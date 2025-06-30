#!/usr/bin/env node

// Simple entry point for Aspire to run the Next.js development server
const { spawn } = require('child_process');
const path = require('path');

// Get the directory where this script is located
const projectDir = __dirname;

// Start the Next.js development server
const child = spawn('npm', ['run', 'dev'], {
  cwd: projectDir,
  stdio: 'inherit',
  shell: true
});

child.on('error', (error) => {
  console.error('Failed to start Next.js:', error);
  process.exit(1);
});

child.on('exit', (code) => {
  process.exit(code);
}); 