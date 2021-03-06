﻿using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleWebApi.Repositories;
using SampleWebApi.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleWebApi_2_2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add User repository to the dependency container.
            services.AddTransient<IApiKeyRepository, InMemoryApiKeyRepository>();

            // Add the ApiKey scheme authentication here..
            // It requires Realm to be set in the options if SuppressWWWAuthenticateHeader is not set.
            // If an implementation of IApiKeyProvider interface is registered in the dependency register as well as OnValidateKey delegete on options.Events is also set then this delegate will be used instead of an implementation of IApiKeyProvider.
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)

                // The below AddApiKeyInHeaderOrQueryParams without type parameter will require OnValidateKey delegete on options.Events to be set unless an implementation of IApiKeyProvider interface is registered in the dependency register.
                // Please note if OnValidateKey delegete on options.Events is also set then this delegate will be used instead of ApiKeyProvider.*
                //.AddApiKeyInHeaderOrQueryParams(options =>

                // The below AddApiKeyInHeaderOrQueryParams with type parameter will add the ApiKeyProvider to the dependency register. 
                // Please note if OnValidateKey delegete on options.Events is also set then this delegate will be used instead of ApiKeyProvider.
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Sample Web API";
                    options.KeyName = "X-API-KEY";

                    //// Optional option to suppress the browser login dialog for ajax calls.
                    //options.SuppressWWWAuthenticateHeader = true;

                    //// Optional events to override the ApiKey original logic with custom logic.
                    //// Only use this if you know what you are doing at your own risk. Any of the events can be assigned. 
                    options.Events = new ApiKeyEvents
                    {

                        //// A delegate assigned to this property will be invoked just before validating the api key. 
                        //OnValidateKey = async (context) =>
                        //{
                        //    // custom code to handle the api key, create principal and call Success method on context.
                        //    var apiKeyRepository = context.HttpContext.RequestServices.GetRequiredService<IApiKeyRepository>();
                        //    var apiKey = await apiKeyRepository.GetApiKeyAsync(context.ApiKey);
                        //    var isValid = apiKey != null && apiKey.Key.Equals(context.ApiKey, StringComparison.OrdinalIgnoreCase);
                        //    if (isValid)
                        //    {
                        //        context.Response.Headers.Add("ValidationCustomHeader", "From OnValidateKey");
                        //        var claims = new[]
                        //        {
                        //            new Claim(ClaimTypes.NameIdentifier, apiKey.OwnerName, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                        //            new Claim(ClaimTypes.Name, apiKey.OwnerName, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                        //            new Claim("CustomClaimType", "Custom Claim Value - from OnValidateKey")
                        //        };
                        //        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        //        context.Success();
                        //    }
                        //    else
                        //    {
                        //        context.NoResult();
                        //    }
                        //},

                        //// A delegate assigned to this property will be invoked just before validating the api key. 
                        //// NOTE: Same as above delegate but slightly different implementation which will give same result.
                        //OnValidateKey = async (context) =>
                        //{
                        //    // custom code to handle the api key, create principal and call Success method on context.
                        //    var apiKeyRepository = context.HttpContext.RequestServices.GetRequiredService<IApiKeyRepository>();
                        //    var apiKey = await apiKeyRepository.GetApiKeyAsync(context.ApiKey);
                        //    var isValid = apiKey != null && apiKey.Key.Equals(context.ApiKey, StringComparison.OrdinalIgnoreCase);
                        //    if (isValid)
                        //    {
                        //        context.Response.Headers.Add("ValidationCustomHeader", "From OnValidateKey");
                        //        var claims = new[]
                        //        {
                        //            new Claim("CustomClaimType", "Custom Claim Value - from OnValidateKey")
                        //        };
                        //        context.ValidationSucceeded(apiKey.OwnerName, claims);    // claims are optional
                        //    }
                        //    else
                        //    {
                        //        context.ValidationFailed();
                        //    }
                        //},

                        //// A delegate assigned to this property will be invoked before a challenge is sent back to the caller when handling unauthorized response.
                        //OnHandleChallenge = async (context) =>
                        //{
                        //    // custom code to handle authentication challenge unauthorized response.
                        //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //    context.Response.Headers.Add("ChallengeCustomHeader", "From OnHandleChallenge");
                        //    await context.Response.WriteAsync("{\"CustomBody\":\"From OnHandleChallenge\"}");
                        //    context.Handled();  // important! do not forget to call this method at the end.
                        //},

                        //// A delegate assigned to this property will be invoked if Authorization fails and results in a Forbidden response.
                        //OnHandleForbidden = async (context) =>
                        //{
                        //    // custom code to handle forbidden response.
                        //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        //    context.Response.Headers.Add("ForbidCustomHeader", "From OnHandleForbidden");
                        //    await context.Response.WriteAsync("{\"CustomBody\":\"From OnHandleForbidden\"}");
                        //    context.Handled();  // important! do not forget to call this method at the end.
                        //},

                        //// A delegate assigned to this property will be invoked when the authentication succeeds. It will not be called if OnValidateKey delegate is assigned.
                        //// It can be used for adding claims, headers, etc to the response.
                        //OnAuthenticationSucceeded = (context) =>
                        //{
                        //    //custom code to add extra bits to the success response.
                        //    context.Response.Headers.Add("SuccessCustomHeader", "From OnAuthenticationSucceeded");
                        //    var customClaims = new List<Claim>
                        //    {
                        //        new Claim("CustomClaimType", "Custom Claim Value - from OnAuthenticationSucceeded")
                        //    };
                        //    context.AddClaims(customClaims);
                        //    //or can add like this - context.Principal.AddIdentity(new ClaimsIdentity(customClaims));
                        //    return Task.CompletedTask;
                        //},

                        //// A delegate assigned to this property will be invoked when the authentication fails.
                        //OnAuthenticationFailed = (context) =>
                        //{
                        //    // custom code to handle failed authentication.
                        //    context.Fail("Failed to authenticate");
                        //    return Task.CompletedTask;
                        //}

                    };
                });

            services.AddMvc(options =>
            {
                // ALWAYS USE HTTPS (SSL) protocol in production when using Basic authentication.
                //options.Filters.Add<RequireHttpsAttribute>();

                // All the requests will need to be authorized. 
                // Alternatively, add [Authorize] attribute to Controller or Action Method where necessary.
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            })
                //.AddXmlSerializerFormatters()   // To enable XML along with JSON
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
