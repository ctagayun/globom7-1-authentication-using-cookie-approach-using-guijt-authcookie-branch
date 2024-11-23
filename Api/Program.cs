using ConfArch.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
//builder.Services.AddCors();

//*Step1: This is out of the box support for CookieAuthentication. Add it to the dependency injection container
//*and configgure COOKIE SCHEME
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>  //*This is the COOKIE SCHEME
    {
        o.Cookie.Name = "__Host-spa"; //*this prefix will prevent subdomains to alter the cookie
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

//*Step5 declare a policy whic requires a claim with type, role, and a value, admin
builder.Services.AddAuthorization(o => 
    o.AddPolicy("admin", p => p.RequireClaim("role", "Admin"))
);

//add the HouseDbContext to the dependency injection container
//I will be registering it with a scope of "Scope" which means a new instance
//will be created for each request that the API will receive.
//Because of that I am turning off a feature of database context that
//tracks each entity instance for property changes. It is more performant
//this way
builder.Services.AddDbContext<HouseDbContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles(); //*Added - serves the content of wwwroot

 //*STEP2: In order to activate the authentication middleware you have to call this method
 //*before endpoint declaration
app.UseAuthentication();

//*Cors no longer needed because requestd come from same domain
//app.UseCors(p => p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());

app.MapHouseEndpoints();
app.MapBidEndpoints();
app.UseRouting();
app.UseAuthorization();  //*Step6: Call authorization middleware
app.MapDefaultControllerRoute();

//*index.html is the entrypoint of out React application
 //*if no endpoint match we tell it to fall back to index.html 
  //*we defined endpoints for Swagger (app.UseSwaggerUI) and 
//*the API app.MapHouseEndpoints and  app.MapBidEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

