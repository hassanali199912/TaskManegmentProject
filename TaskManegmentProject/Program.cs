using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskManegmentProject.DBcontcion;
using TaskManegmentProject.Repos;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews(
   options =>
{
   var policy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       .Build();
   options.Filters.Add(new AuthorizeFilter(policy));
}).AddRazorRuntimeCompilation();

//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddSession(op =>
{
    op.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<TMContextDB>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(op =>
{
    op.Password.RequireDigit = false;
    op.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<TMContextDB>();


builder.Services.AddScoped<ITaskRepository,TaskRepository>();
builder.Services.AddScoped<IWorkSpaceRepository, WorkSpaceRepository>();
builder.Services.AddScoped<IMemberWorkSpaceRepository, MemberWorkSpaceRepository>();
builder.Services.AddScoped<IMessageChatRepository, MessageChatRepository>();

//builder.Services.AddScoped<IWorkSpaceRepository, WorkSpaceRepository>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
