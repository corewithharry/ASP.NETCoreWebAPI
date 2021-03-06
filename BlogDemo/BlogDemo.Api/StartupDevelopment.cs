﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlogDemo.Api.Extensions;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DataBase;
using BlogDemo.Infrastructure.Repositories;
using BlogDemo.Infrastructure.Services;
using BlogDemo.Infrastructure.ViewModel;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        public static IConfiguration Configuration { get; set; }

        public StartupDevelopment(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options=>
            {
                options.ReturnHttpNotAcceptable = true;
                //options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                var inputFromatter = options.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault();
                if (inputFromatter != null)
                { 
                    inputFromatter.SupportedMediaTypes.Add("application/vnd.hy.post.create+json");                    
                    inputFromatter.SupportedMediaTypes.Add("application/vnd.hy.post.update+json");
                    //inputFromatter.SupportedMediaTypes.Add("application/json-patch+json");
                }

                var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                if(outputFormatter!=null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.hy.hateoas+json");
                }
            })
            .AddJsonOptions(options=>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation();

            services.AddDbContext<MyContext>(options =>
            {
                var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                options.UseSqlite(connectionString);
            });

            services.AddHttpsRedirection(options=>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;

            });


            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper();

            services.AddTransient<IValidator<PostViewModel>, PostViewModelValidator>();
            services.AddTransient<IValidator<PostAddViewModel> , PostAddOrUpdateViewModelValidator<PostAddViewModel>>();
            services.AddTransient<IValidator<PostUpdateViewModel>, PostAddOrUpdateViewModelValidator<PostUpdateViewModel>>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<PostPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);

            services.AddTransient<ITypeHelperService, TypeHelperService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            //app.UseDeveloperExceptionPage();
            app.UseMyExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
