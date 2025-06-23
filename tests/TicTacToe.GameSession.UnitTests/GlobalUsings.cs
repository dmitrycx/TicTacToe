// Test Framework
global using Xunit;

// Domain
global using TicTacToe.GameSession.Domain.Entities;
global using TicTacToe.GameSession.Domain.Enums;
global using TicTacToe.GameSession.Domain.Events;
global using TicTacToe.GameSession.Domain.Exceptions;

// Infrastructure
global using TicTacToe.GameSession.Infrastructure.Configuration;
global using TicTacToe.GameSession.Infrastructure.Persistence;

// Game Engine (for cross-domain references)
global using TicTacToe.GameEngine.Domain.Enums;
global using TicTacToe.GameEngine.Domain.ValueObjects;

// System
global using System.ComponentModel.DataAnnotations; 