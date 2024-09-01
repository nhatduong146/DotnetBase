using DotnetBase.Media.Extensions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.BuildConfiguration();

builder.ConfigureServices();

builder.BuildAndRun();
