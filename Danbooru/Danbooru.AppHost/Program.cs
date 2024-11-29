var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Danbooru_UI>("danbooru-ui");

builder.Build().Run();
