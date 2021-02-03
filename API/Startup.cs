using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Video 56
            // Relocated to ApplicationServicesExtensions
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddControllers();

            services.AddDbContext<StoreContext>(x => x.UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IConnectionMultiplexer>(c => {
                var configuration = ConfigurationOptions.Parse(_config
                .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            
            // Video 53. this is added after "services.AddControllers();"
            // Video 56
            // Relocated to ApplicationServicesExtensions
            // services.Configure<ApiBehaviorOptions>(options => 
            // {
            //    options.InvalidModelStateResponseFactory = actionContext =>
            //    {
            //        var errors = actionContext.ModelState
            //                 .Where(e => e.Value.Errors.Count > 0)
            //                 .SelectMany(x => x.Value.Errors)
            //                 .Select(x => x.ErrorMessage).ToArray();
            //         var errorResponse = new ApiValidationErrorResponse
            //         {
            //             Errors = errors
            //         };
            //         return new BadRequestObjectResult(errorResponse);        
            //    };     
            // }); 
             // Video 56
             services.AddApplicationServices();   
                services.AddSwaggerDocumentation();
             services.AddCors(opt =>
             {
                 opt.AddPolicy("CorsPolicy", policy =>
                 {
                     policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                 });
             })   ;
            // Video 54
            // Nuget add 'swashbukcle.aspnetcore - SwaggerGen then
            // Nuget add 'swashbukcle.aspnetcore - SwaggerUI 
            // then add app.UseSwagger() to Configure

            // Video 56 : relocated to SwaggerServiceExtensions
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo{ Title = "SkiNet API", Version = "v1"});
            // });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Video 52 
            app.UseMiddleware<ExceptionMiddleware>();
            // Video 52
            
            // Video 51
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            //Video 51 
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();


            // Video 54
            //Video 56 : relocated to SwaggerServiceExtension
            // app.UseSwagger();
            // app.UseSwaggerUI(c => {c
            //     .SwaggerEndpoint("/swagger/v1/swagger.json", "SkiNet API v1");});
            app.UseSwaggerDocumentation();
             //Video 54 ends   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
