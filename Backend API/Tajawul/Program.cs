using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tajawul.Data;
using Tajawul.Interfaces;
using Tajawul.Models.Domain;
using Tajawul.Repositories;
using Tajawul.Services;
using Tajawul.Services.Reviews;
using Tajawul.Services.Destinations;
using Tajawul.Interfaces.Destinations;
using Tajawul.Repositories.Destinations;
using Tajawul.Repositories.User.Interaction;
using Tajawul.Interfaces.User.Interactions;
using Tajawul.Services.User.Profile.Interactions;
using Tajawul.Services.BackgroundServices;
using Tajawul.Repositories.user.Profile;
using Tajawul.Interfaces.Media;
using Tajawul.Services.UploadService;
using Tajawul.Services.User.Profile;
using Tajawul.Interfaces.User.Profile;
using Tajawul.Repositories.User;
using Tajawul.Interfaces.User.ChatBot;
using Tajawul.Services.User.Chatbot;
using Tajawul.Data.Configuration;
using Tajawul.Interfaces.Events;
using Tajawul.Repositories.Events;
using Tajawul.Services.Events;
using Tajawul.Services.User.Interactions;
using Tajawul.Interfaces.Trips;
using Tajawul.Repositories.Trips;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Services.Weather;
using Tajawul.Services.SocialMedia;
using Tajawul.Repositories.SocialMedia;
using Tajawul.Interfaces.SocialMedia;
using Tajawul.Interfaces.General;
using Tajawul.Interfaces.User.Translation;
using Tajawul.Interfaces.Reviews;
using Tajawul.Models.Domain.ExternalAPIs;
using Tajawul.Models.Domain.Users;
using Tajawul.Repositories.General;
using Tajawul.Repositories.SocialMedia.Posts;
using Tajawul.Services.SocialMedia.Posts;
using Tajawul.Services.User.Translation;
using Tajawul.Data.Configuration.MongoConfiguration;
using Tajawul.Services.user.Translation;
using Tajawul.Repositories.user;
using Tajawul.Interfaces.Recommendation;
using Tajawul.Services.Recommendation;
public class Program
{
    public static void Main(string[] args)
    {

        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.AllowAnyOrigin();
                }
            );
        });

        builder.Services.AddDbContext<Db14639Context>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //Configure Neo4j
        builder.Services.Configure<Neo4jSettings>(builder.Configuration.GetSection("Neo4jSettings"));
        builder.Services.AddSingleton<Neo4jService>();

        // Register MongoDB services using MongoConfiguration
        builder.Services.AddMongoDbServices(builder.Configuration);


        builder.Services.Configure<LLMOptions>(builder.Configuration.GetSection("LLM"));
        builder.Services.Configure<MTOptions>(builder.Configuration.GetSection("MT"));
        builder.Services.Configure<CurrencyConverter>(builder.Configuration.GetSection("ApiSettings"));

        CurrencyExistsAttribute.LoadSupportedCurrencies(builder.Environment.ContentRootPath);

        DestinationTypeExistsAttribute.LoadSupportedDestinationTypes(builder.Environment.ContentRootPath);

        TranslationLanguageCodeExits.LoadSupportedLanguageCodes(builder.Environment.ContentRootPath);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("FullyRegisteredUser", policy =>
                policy.RequireRole("User"));

            options.AddPolicy("PlaceOwner", policy =>
                policy.RequireRole("PlaceOwner"));

            //options.AddPolicy("SocialOrInterestOrUser", policy =>
            //    policy.RequireRole("CompletedSocialInfo", "CompletedInterestInfo", "User"));
        });

        builder.Services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = 3;
                options.Window = TimeSpan.FromSeconds(5);
                options.QueueLimit = 0;
            });

            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        builder.Services.AddIdentity<Person, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        }).AddEntityFrameworkStores<Db14639Context>()
        .AddDefaultTokenProviders();

        // Configure token lifespan
        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            // Set token lifespan to 2 hours
            options.TokenLifespan = TimeSpan.FromHours(2);
        });

        builder.Services.AddAuthentication(options =>
        {

            options.DefaultAuthenticateScheme =
            options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
            options.DefaultScheme =
            options.DefaultSignInScheme =
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8
                    .GetBytes(builder.Configuration["JWT:SigningKey"]!)
                ),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Tajawul", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        //Configure Email Settings
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddSingleton<EmailBackgroundService>();
        builder.Services.AddHostedService(provider => provider.GetRequiredService<EmailBackgroundService>());

        builder.Services.AddScoped<ITokenService, TokenService>();

        builder.Services.AddScoped<ReviewRepository>();
        builder.Services.AddScoped<IReviewService, ReviewService>();

        builder.Services.AddScoped<DestinationRepository>();
        builder.Services.AddScoped<ActivityRepository>();
        builder.Services.AddScoped<GroupSizeRepository>();
        builder.Services.AddScoped<PriceRangeRepository>();
        builder.Services.AddScoped<TagRepository>();
        builder.Services.AddScoped<TypeRepository>();
        builder.Services.AddScoped<OpenCloseRepository>();
        builder.Services.AddScoped<DLocationRepository>();
        builder.Services.AddScoped<CommentRepository>();
        builder.Services.AddScoped<VotingRepository>();
        builder.Services.AddScoped<UserInteractionsRepository>();


        builder.Services.AddScoped<IDestinationService, DestinationService>();
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<IGroupSizeService, GroupSizeService>();
        builder.Services.AddScoped<IPriceRangeService, PriceRangeService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<ITypeService, TypeService>();
        builder.Services.AddScoped<IOpenCloseService, OpenCloseService>();
        builder.Services.AddScoped<IDLocationService, DLocationService>();
        builder.Services.AddScoped<CurrencyConverterService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IUserInteractionsService, UserInteractionsService>();

        builder.Services.AddScoped<LocationRepository>();
        builder.Services.AddScoped<ILocationService, LocationService>();

        builder.Services.AddScoped<InterestRepository>();
        builder.Services.AddScoped<IInterestService, InterestService>();

        builder.Services.AddScoped<MaritalStatusRepository>();
        builder.Services.AddScoped<IMaritalStatusService, MaritalStatusService>();

        builder.Services.AddScoped<SpokenLanguageRepository>();
        builder.Services.AddScoped<ISpokenLanguageService, SpokenLanguageService>();

        builder.Services.AddScoped<TripDurationRepository>();
        builder.Services.AddScoped<ITripDurationService, TripDurationService>();

        builder.Services.AddScoped<StatusRepository>();
        builder.Services.AddScoped<IStatusService, StatusService>();

        builder.Services.AddScoped<VisibilityRepository>();
        builder.Services.AddScoped<IVisibilityService, VisibilityService>();

        builder.Services.AddScoped<EventDateRangeRepository>();
        builder.Services.AddScoped<IEventDateRangeService, EventDateRangeService>();

        builder.Services.AddScoped<EventRepository>();
        builder.Services.AddScoped<IEventService, EventService>();

        builder.Services.AddScoped<TripRepository>();
        builder.Services.AddScoped<ITripService, TripService>();

        builder.Services.AddScoped<UserProfileRepository>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<ChatbotRepository>();
        builder.Services.AddScoped<IChatbotService, ChatbotService>();

        builder.Services.AddHttpClient<LlmService>();
        builder.Services.AddScoped<ILlmService, LlmService>();

        builder.Services.AddScoped<TranslationRepository>();
        builder.Services.AddScoped<ITranslationService, TranslationService>();
        builder.Services.AddScoped<IMTService, MTService>();

        builder.Services.AddScoped<SearchBarRepository>();
        builder.Services.AddScoped<ISearchService, SearchService>();

        builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

        // Social Media
        builder.Services.AddScoped<PostRepository>();
        builder.Services.AddScoped<IPostsService, PostsService>();

        // Interactions
        builder.Services.AddScoped<DestinationInteractionsRepository>();
        builder.Services.AddScoped<IDestinationInteractionsService, DestinationInteractionsService>();

        builder.Services.AddScoped<TripInteractionsRepository>();
        builder.Services.AddScoped<ITripInteractionsService, TripInteractionsService>();

        builder.Services.AddScoped<RecommendationRepository>();
        builder.Services.AddScoped<IRecommendationService, RecommendationService>();

        builder.Services.AddScoped<EventInteractionsRepository>();
        builder.Services.AddScoped<IEventInteractionsService, EventInteractionsService>();

        // Background Services
        builder.Services.AddHostedService<RatingBackgroundService>();
        builder.Services.AddHostedService<UpdateDestinationCountersBackgroundService>();

        // Upload Service
        builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

        // Weather Service
        builder.Services.AddHttpClient<VCWeatherService>();
        builder.Services.Configure<VCWeatherConfiguration>(builder.Configuration.GetSection("VCWeather"));


        var app = builder.Build();

        app.UseRouting();

        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.UseCors(MyAllowSpecificOrigins);

        app.MapControllers();

        app.Run();

    }
}