// Global Usings for TicTacToe.GameSession.Tests project

#region Testing Framework
global using Xunit;
global using FluentAssertions;
global using Moq;
#endregion

#region Domain
global using TicTacToe.GameSession.Domain.Aggregates;
global using TicTacToe.GameSession.Domain.Entities;
global using TicTacToe.GameSession.Domain.Enums;
global using TicTacToe.GameSession.Domain.Events;
global using TicTacToe.GameSession.Domain.Exceptions;
global using TicTacToe.GameSession.Domain.Services;
#endregion

#region Infrastructure
global using TicTacToe.GameSession.Infrastructure.External;
global using TicTacToe.GameSession.Persistence;
#endregion

#region System & Framework
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Net;
global using System.Text;
global using System.Text.Json;
#endregion 