﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.DependencyInjection;
using MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.Server.GrpcServer;
using MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.Server.Infrastructure.Grpc;
using MorganStanley.ComposeUI.ProcessExplorer.Abstractions.Infrastructure;
using MorganStanley.ComposeUI.ProcessExplorer.Core.DependencyInjection;
using System.Diagnostics;
using MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.Sever.Abstractions;

namespace MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.Server.Abstractions;

public static class ProcessExplorerBuilderExtensions
{
    public static ProcessExplorerBuilder UseGrpc(
        this ProcessExplorerBuilder builder,
        Action<ProcessExplorerServerOptions>? options = null)
    {
        if (options != null) builder.ServiceCollection.Configure(options);

        builder.ServiceCollection.AddGrpc();
        builder.ServiceCollection.AddCors(o => o.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        builder.ServiceCollection.Configure<ProcessExplorerServerOptions>(op =>
        {
            op.Port = 5060;
            op.MainProcessId = Process.GetCurrentProcess().Id;
            op.EnableProcessExplorer = true;
        });

        builder.ServiceCollection.AddProcessExplorerAggregator();
        builder.ServiceCollection.AddProcessMonitorWindows();
        builder.ServiceCollection.AddSubsystemController();
        builder.ServiceCollection.AddSingleton<IUiHandler, GrpcUiHandler>();

        builder.ServiceCollection.AddSingleton<GrpcListenerService>();
        builder.ServiceCollection.AddSingleton<IHostedService>(provider => provider.GetRequiredService<GrpcListenerService>());
        builder.ServiceCollection.AddSingleton<ProcessExplorerServer>(provider => provider.GetRequiredService<GrpcListenerService>());

        return builder;
    }
}