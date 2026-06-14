using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Prospector.Web.Models;

namespace Prospector.Web.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.EnsureCreatedAsync();

        // Ensure Admin role exists
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // Seed admin user
        if (!await db.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@prospector.com",
                Email = "admin@prospector.com",
                DisplayName = "Admin Scout",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");

            var scout = new ApplicationUser
            {
                UserName = "scout@prospector.com",
                Email = "scout@prospector.com",
                DisplayName = "Analista de Prospectos",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(scout, "Scout@123");
        }

        // Assign Admin role to existing admin user if not yet assigned (migration path)
        var existingAdmin = await userManager.FindByEmailAsync("admin@prospector.com");
        if (existingAdmin != null && !await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            await userManager.AddToRoleAsync(existingAdmin, "Admin");

        if (await db.Players.AnyAsync()) return;

        var players = new List<Player>
        {
            new() {
                Name = "Jackson Montgomery",
                School = "Alabama State University",
                Conference = "SEC",
                Position = "QB",
                DraftClassYear = 2026,
                HeightInches = 75, // 6'3"
                WeightLbs = 225,
                HomeTown = "Birmingham",
                HomeState = "AL",
                JerseyNumber = 1,
                ProjectedRound = 1,
                ProjectedPick = 3,
                OverallGrade = 89.0,
                CeilingGrade = 95.0,
                FloorGrade = 72.0,
                NflComparison = "Lamar Jackson",
                Bio = "Elite dual-threat prospect com combinação única de braço forte e atletismo explosivo. Lidera ASU em produção total em todas as categorias. Considerado o QB #1 da classe 2026.",
                SeasonStats = [
                    new() {
                        Year = 2022, Season = "Regular", GamesPlayed = 10, GamesStarted = 8,
                        Attempts = 198, Completions = 124, PassingYards = 1680, PassingTDs = 14,
                        Interceptions = 6, Sacks = 12, SackYardsLost = 78,
                        RushingAttempts = 98, RushingYards = 612, RushingTDs = 8,
                        CompletionPct = 62.6, YardsPerAttempt = 8.5, AdjYardsPerAttempt = 8.1,
                        PasserRating = 148.2, Qbr = 74.3, Epa = 48.2, EpaPerPlay = 0.18,
                        Cpoe = 2.1, PressureRate = 0.28, SackRate = 0.057, OnTargetThrowPct = 72.4,
                        ThirdDownConvPct = 41.2, RedZoneCompPct = 58.3, DropPct = 0.04
                    },
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 312, Completions = 208, PassingYards = 2940, PassingTDs = 26,
                        Interceptions = 8, Sacks = 18, SackYardsLost = 112,
                        RushingAttempts = 142, RushingYards = 892, RushingTDs = 11,
                        CompletionPct = 66.7, YardsPerAttempt = 9.4, AdjYardsPerAttempt = 9.1,
                        PasserRating = 162.4, Qbr = 84.7, Epa = 98.6, EpaPerPlay = 0.27,
                        Cpoe = 4.2, PressureRate = 0.24, SackRate = 0.054, OnTargetThrowPct = 74.8,
                        ThirdDownConvPct = 46.8, RedZoneCompPct = 63.2, DropPct = 0.03
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 358, Completions = 245, PassingYards = 3892, PassingTDs = 37,
                        Interceptions = 7, Sacks = 22, SackYardsLost = 148,
                        RushingAttempts = 168, RushingYards = 1124, RushingTDs = 14,
                        CompletionPct = 68.4, YardsPerAttempt = 10.9, AdjYardsPerAttempt = 10.6,
                        PasserRating = 174.8, Qbr = 91.2, Epa = 142.3, EpaPerPlay = 0.34,
                        Cpoe = 6.1, PressureRate = 0.21, SackRate = 0.058, OnTargetThrowPct = 77.2,
                        ThirdDownConvPct = 52.4, RedZoneCompPct = 68.9, DropPct = 0.028
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 15),
                        ArmStrength = 88, Accuracy = 84, DeepBallAccuracy = 85, Footwork = 80,
                        Mechanics = 79, DecisionMaking = 82, PocketPresence = 80, MobilityAgility = 96,
                        Leadership = 90, FootballIq = 86, ReleaseSpeed = 84,
                        Strengths = "Elite athleticism, arm strength excepcional fundo do campo, liderança natural, improvisação fora do pocket",
                        Weaknesses = "Pode depender demais da mobilidade em vez de trabalhar progressões, mecânica inconsistente sob pressão",
                        NflComparison = "Lamar Jackson"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "Pro Day", MeasuredAt = new DateTime(2025, 3, 12),
                        FortyYardDash = 4.38, TwentyYardSplit = 2.54, TenYardSplit = 1.51,
                        VerticalJump = 38.5, BroadJump = 124, ThreeConeDrill = 6.72,
                        ShuttleRun = 4.06, HandSizeInches = 9.75, ArmLengthInches = 33.5, WingSpanInches = 78.2
                    }
                ]
            },

            new() {
                Name = "Ethan Chambers",
                School = "Ohio Northern University",
                Conference = "Big Ten",
                Position = "QB",
                DraftClassYear = 2025,
                HeightInches = 76, // 6'4"
                WeightLbs = 232,
                HomeTown = "Columbus",
                HomeState = "OH",
                JerseyNumber = 12,
                ProjectedRound = 1,
                ProjectedPick = 1,
                OverallGrade = 92.0,
                CeilingGrade = 94.0,
                FloorGrade = 80.0,
                NflComparison = "Trevor Lawrence",
                Bio = "Pocket passer clássico com precisão de elite e leitura de defesa excepcional. Cinco anos no sistema universitário o tornaram o prospecto mais polido da classe 2025.",
                SeasonStats = [
                    new() {
                        Year = 2022, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 398, Completions = 276, PassingYards = 3420, PassingTDs = 28,
                        Interceptions = 9, Sacks = 20, SackYardsLost = 128,
                        RushingAttempts = 48, RushingYards = 168, RushingTDs = 2,
                        CompletionPct = 69.3, YardsPerAttempt = 8.6, AdjYardsPerAttempt = 8.2,
                        PasserRating = 156.8, Qbr = 81.4, Epa = 112.4, EpaPerPlay = 0.24,
                        Cpoe = 5.1, PressureRate = 0.22, SackRate = 0.048, OnTargetThrowPct = 76.2,
                        ThirdDownConvPct = 48.3, RedZoneCompPct = 66.1, DropPct = 0.032
                    },
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 424, Completions = 302, PassingYards = 3890, PassingTDs = 36,
                        Interceptions = 7, Sacks = 16, SackYardsLost = 98,
                        RushingAttempts = 42, RushingYards = 142, RushingTDs = 1,
                        CompletionPct = 71.2, YardsPerAttempt = 9.2, AdjYardsPerAttempt = 9.1,
                        PasserRating = 168.2, Qbr = 88.6, Epa = 138.2, EpaPerPlay = 0.30,
                        Cpoe = 7.4, PressureRate = 0.19, SackRate = 0.036, OnTargetThrowPct = 78.9,
                        ThirdDownConvPct = 52.1, RedZoneCompPct = 71.4, DropPct = 0.025
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 14, GamesStarted = 14,
                        Attempts = 468, Completions = 338, PassingYards = 4122, PassingTDs = 42,
                        Interceptions = 6, Sacks = 14, SackYardsLost = 88,
                        RushingAttempts = 38, RushingYards = 124, RushingTDs = 2,
                        CompletionPct = 72.2, YardsPerAttempt = 8.8, AdjYardsPerAttempt = 9.0,
                        PasserRating = 172.4, Qbr = 92.4, Epa = 158.8, EpaPerPlay = 0.38,
                        Cpoe = 8.2, PressureRate = 0.18, SackRate = 0.029, OnTargetThrowPct = 80.1,
                        ThirdDownConvPct = 56.8, RedZoneCompPct = 73.2, DropPct = 0.022
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 8),
                        ArmStrength = 85, Accuracy = 92, DeepBallAccuracy = 90, Footwork = 85,
                        Mechanics = 91, DecisionMaking = 92, PocketPresence = 90, MobilityAgility = 60,
                        Leadership = 88, FootballIq = 93, ReleaseSpeed = 88,
                        Strengths = "Precisão cirúrgica em todas as janelas, leitura pré-snap excepcional, manipulação de safeties, tomada de decisão sob pressão",
                        Weaknesses = "Mobilidade limitada, pode ser vulnerável a pressão de blitz com edge rushers rápidos",
                        NflComparison = "Trevor Lawrence"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "NFL Combine", MeasuredAt = new DateTime(2025, 2, 28),
                        FortyYardDash = 4.72, TwentyYardSplit = 2.71, TenYardSplit = 1.62,
                        VerticalJump = 32.0, BroadJump = 108, ThreeConeDrill = 7.12,
                        ShuttleRun = 4.28, HandSizeInches = 10.125, ArmLengthInches = 33.75, WingSpanInches = 79.5
                    }
                ]
            },

            new() {
                Name = "Marcus Webb",
                School = "Florida Southern University",
                Conference = "ACC",
                Position = "QB",
                DraftClassYear = 2026,
                HeightInches = 73, // 6'1"
                WeightLbs = 207,
                HomeTown = "Miami",
                HomeState = "FL",
                JerseyNumber = 7,
                ProjectedRound = 1,
                ProjectedPick = 8,
                OverallGrade = 78.0,
                CeilingGrade = 93.0,
                FloorGrade = 58.0,
                NflComparison = "Kyler Murray",
                Bio = "Prospecto de alto teto com atletismo de outro nível. Mecânica ainda em desenvolvimento, mas potencial para ser um transformador de franquia com o coaching certo.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 11, GamesStarted = 10,
                        Attempts = 248, Completions = 148, PassingYards = 1920, PassingTDs = 18,
                        Interceptions = 9, Sacks = 24, SackYardsLost = 168,
                        RushingAttempts = 128, RushingYards = 748, RushingTDs = 9,
                        CompletionPct = 59.7, YardsPerAttempt = 7.7, AdjYardsPerAttempt = 7.1,
                        PasserRating = 138.4, Qbr = 71.2, Epa = 62.1, EpaPerPlay = 0.18,
                        Cpoe = -2.8, PressureRate = 0.32, SackRate = 0.088, OnTargetThrowPct = 68.4,
                        ThirdDownConvPct = 38.6, RedZoneCompPct = 55.8, DropPct = 0.042
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 312, Completions = 191, PassingYards = 2984, PassingTDs = 28,
                        Interceptions = 9, Sacks = 28, SackYardsLost = 198,
                        RushingAttempts = 162, RushingYards = 1048, RushingTDs = 12,
                        CompletionPct = 61.2, YardsPerAttempt = 9.6, AdjYardsPerAttempt = 9.0,
                        PasserRating = 148.9, Qbr = 79.3, Epa = 96.4, EpaPerPlay = 0.25,
                        Cpoe = -0.4, PressureRate = 0.28, SackRate = 0.082, OnTargetThrowPct = 71.2,
                        ThirdDownConvPct = 43.2, RedZoneCompPct = 59.4, DropPct = 0.038
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 2, 1),
                        ArmStrength = 82, Accuracy = 71, DeepBallAccuracy = 76, Footwork = 72,
                        Mechanics = 68, DecisionMaking = 70, PocketPresence = 68, MobilityAgility = 94,
                        Leadership = 78, FootballIq = 72, ReleaseSpeed = 78,
                        Strengths = "Atletismo transcendente, burst e aceleração de WR, lança bem em movimento, competitivo e melhora a cada ano",
                        Weaknesses = "Mecânica inconsistente no pocket estático, progressões limitadas, sack rate preocupante",
                        NflComparison = "Kyler Murray"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "Pro Day", MeasuredAt = new DateTime(2026, 3, 10),
                        FortyYardDash = 4.42, TwentyYardSplit = 2.56, TenYardSplit = 1.52,
                        VerticalJump = 41.0, BroadJump = 128, ThreeConeDrill = 6.68,
                        ShuttleRun = 3.98, HandSizeInches = 9.25, ArmLengthInches = 31.5, WingSpanInches = 74.8
                    }
                ]
            },

            new() {
                Name = "Ryan Pierce",
                School = "Dakota Tech University",
                Conference = "Mountain West",
                Position = "QB",
                DraftClassYear = 2025,
                HeightInches = 74, // 6'2"
                WeightLbs = 218,
                HomeTown = "Sioux Falls",
                HomeState = "SD",
                JerseyNumber = 14,
                ProjectedRound = 2,
                ProjectedPick = 42,
                OverallGrade = 72.0,
                CeilingGrade = 82.0,
                FloorGrade = 62.0,
                NflComparison = "Gardner Minshew",
                Bio = "Produtor consistente em escola de menor nível. Números impressionantes mas enfrentam questionamentos sobre o nível de competição. Mecânica sólida e alta inteligência.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 12, GamesStarted = 12,
                        Attempts = 388, Completions = 276, PassingYards = 3680, PassingTDs = 38,
                        Interceptions = 7, Sacks = 14, SackYardsLost = 88,
                        RushingAttempts = 62, RushingYards = 248, RushingTDs = 4,
                        CompletionPct = 71.1, YardsPerAttempt = 9.5, AdjYardsPerAttempt = 9.4,
                        PasserRating = 168.8, Qbr = 82.1, Epa = 128.4, EpaPerPlay = 0.28,
                        Cpoe = 6.8, PressureRate = 0.16, SackRate = 0.035, OnTargetThrowPct = 77.8,
                        ThirdDownConvPct = 52.8, RedZoneCompPct = 70.2, DropPct = 0.028
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 428, Completions = 318, PassingYards = 4562, PassingTDs = 51,
                        Interceptions = 4, Sacks = 10, SackYardsLost = 62,
                        RushingAttempts = 72, RushingYards = 312, RushingTDs = 6,
                        CompletionPct = 74.3, YardsPerAttempt = 10.7, AdjYardsPerAttempt = 11.0,
                        PasserRating = 182.6, Qbr = 88.1, Epa = 164.2, EpaPerPlay = 0.34,
                        Cpoe = 8.4, PressureRate = 0.14, SackRate = 0.023, OnTargetThrowPct = 79.4,
                        ThirdDownConvPct = 58.4, RedZoneCompPct = 74.8, DropPct = 0.022
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 20),
                        ArmStrength = 72, Accuracy = 86, DeepBallAccuracy = 78, Footwork = 78,
                        Mechanics = 82, DecisionMaking = 84, PocketPresence = 80, MobilityAgility = 75,
                        Leadership = 82, FootballIq = 82, ReleaseSpeed = 80,
                        Strengths = "Precisão e tomada de decisão acima da média, nunca joga além das suas possibilidades, gerenciamento eficiente do jogo",
                        Weaknesses = "Competição de nível inferior questionável, braço não excepcional para o NFL, atletismo mediano",
                        NflComparison = "Gardner Minshew"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "NFL Combine", MeasuredAt = new DateTime(2025, 3, 1),
                        FortyYardDash = 4.61, TwentyYardSplit = 2.64, TenYardSplit = 1.58,
                        VerticalJump = 34.5, BroadJump = 114, ThreeConeDrill = 6.98,
                        ShuttleRun = 4.18, HandSizeInches = 9.875, ArmLengthInches = 32.75, WingSpanInches = 76.4
                    }
                ]
            },

            new() {
                Name = "Isaiah Torres",
                School = "Texas Central University",
                Conference = "Big 12",
                Position = "QB",
                DraftClassYear = 2026,
                HeightInches = 75, // 6'3"
                WeightLbs = 215,
                HomeTown = "Houston",
                HomeState = "TX",
                JerseyNumber = 3,
                ProjectedRound = 1,
                ProjectedPick = 12,
                OverallGrade = 71.0,
                CeilingGrade = 91.0,
                FloorGrade = 48.0,
                NflComparison = "Justin Fields (pré-draft)",
                Bio = "Prospecto mais polarizador da classe 2026. Físico impressionante e atletismo de primeira rodada, mas consistência e leitura de defesa precisam evoluir muito.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 10, GamesStarted = 8,
                        Attempts = 198, Completions = 112, PassingYards = 1548, PassingTDs = 14,
                        Interceptions = 10, Sacks = 28, SackYardsLost = 198,
                        RushingAttempts = 118, RushingYards = 682, RushingTDs = 8,
                        CompletionPct = 56.6, YardsPerAttempt = 7.8, AdjYardsPerAttempt = 6.8,
                        PasserRating = 124.8, Qbr = 62.4, Epa = 28.4, EpaPerPlay = 0.09,
                        Cpoe = -5.2, PressureRate = 0.38, SackRate = 0.124, OnTargetThrowPct = 64.2,
                        ThirdDownConvPct = 34.8, RedZoneCompPct = 50.2, DropPct = 0.052
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 298, Completions = 174, PassingYards = 2648, PassingTDs = 22,
                        Interceptions = 11, Sacks = 32, SackYardsLost = 228,
                        RushingAttempts = 148, RushingYards = 928, RushingTDs = 11,
                        CompletionPct = 58.4, YardsPerAttempt = 8.9, AdjYardsPerAttempt = 8.2,
                        PasserRating = 136.2, Qbr = 71.5, Epa = 68.4, EpaPerPlay = 0.17,
                        Cpoe = -2.4, PressureRate = 0.34, SackRate = 0.097, OnTargetThrowPct = 66.8,
                        ThirdDownConvPct = 38.4, RedZoneCompPct = 52.8, DropPct = 0.048
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 2, 15),
                        ArmStrength = 91, Accuracy = 62, DeepBallAccuracy = 70, Footwork = 68,
                        Mechanics = 64, DecisionMaking = 65, PocketPresence = 65, MobilityAgility = 95,
                        Leadership = 75, FootballIq = 67, ReleaseSpeed = 82,
                        Strengths = "Braço de canhão, velocidade e explosão excepcionais, potencial para jogar em sistemas modernos spread",
                        Weaknesses = "Leitura de cobertura muito básica, sack rate alarmante, precisão inconsistente especialmente sob pressão",
                        NflComparison = "Justin Fields (pré-draft)"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "Pro Day", MeasuredAt = new DateTime(2026, 3, 18),
                        FortyYardDash = 4.41, TwentyYardSplit = 2.55, TenYardSplit = 1.50,
                        VerticalJump = 40.5, BroadJump = 126, ThreeConeDrill = 6.71,
                        ShuttleRun = 4.02, HandSizeInches = 9.5, ArmLengthInches = 33.0, WingSpanInches = 77.4
                    }
                ]
            },

            new() {
                Name = "Tyler Brooks",
                School = "Oregon Pacific University",
                Conference = "Pac-12",
                Position = "QB",
                DraftClassYear = 2025,
                HeightInches = 73, // 6'1"
                WeightLbs = 210,
                HomeTown = "Portland",
                HomeState = "OR",
                JerseyNumber = 10,
                ProjectedRound = 2,
                ProjectedPick = 38,
                OverallGrade = 81.0,
                CeilingGrade = 87.0,
                FloorGrade = 68.0,
                NflComparison = "Jalen Hurts",
                Bio = "Transferência de sucesso que floresceu no sistema de Oregon Pacific. Versatilidade como corredor e passador. História de superação e resilência.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 12, GamesStarted = 12,
                        Attempts = 298, Completions = 188, PassingYards = 2680, PassingTDs = 24,
                        Interceptions = 9, Sacks = 22, SackYardsLost = 148,
                        RushingAttempts = 128, RushingYards = 712, RushingTDs = 8,
                        CompletionPct = 63.1, YardsPerAttempt = 9.0, AdjYardsPerAttempt = 8.6,
                        PasserRating = 148.2, Qbr = 77.4, Epa = 88.4, EpaPerPlay = 0.22,
                        Cpoe = 1.2, PressureRate = 0.26, SackRate = 0.069, OnTargetThrowPct = 72.8,
                        ThirdDownConvPct = 44.2, RedZoneCompPct = 61.4, DropPct = 0.036
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 342, Completions = 225, PassingYards = 3448, PassingTDs = 31,
                        Interceptions = 8, Sacks = 18, SackYardsLost = 118,
                        RushingAttempts = 142, RushingYards = 848, RushingTDs = 10,
                        CompletionPct = 65.8, YardsPerAttempt = 10.1, AdjYardsPerAttempt = 9.8,
                        PasserRating = 158.6, Qbr = 83.7, Epa = 118.6, EpaPerPlay = 0.29,
                        Cpoe = 3.4, PressureRate = 0.22, SackRate = 0.050, OnTargetThrowPct = 74.6,
                        ThirdDownConvPct = 48.8, RedZoneCompPct = 64.8, DropPct = 0.031
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 22),
                        ArmStrength = 78, Accuracy = 79, DeepBallAccuracy = 74, Footwork = 76,
                        Mechanics = 74, DecisionMaking = 80, PocketPresence = 77, MobilityAgility = 88,
                        Leadership = 82, FootballIq = 80, ReleaseSpeed = 78,
                        Strengths = "Excelente atleta, capacidade de estender jogadas com os pés, melhora constante de temporada para temporada",
                        Weaknesses = "Estatura abaixo do ideal para o NFL, braço não elite, pode forçar arremessos em janelas fechadas",
                        NflComparison = "Jalen Hurts"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "NFL Combine", MeasuredAt = new DateTime(2025, 2, 27),
                        FortyYardDash = 4.48, TwentyYardSplit = 2.60, TenYardSplit = 1.55,
                        VerticalJump = 37.0, BroadJump = 119, ThreeConeDrill = 6.88,
                        ShuttleRun = 4.12, HandSizeInches = 9.625, ArmLengthInches = 32.125, WingSpanInches = 75.6
                    }
                ]
            },

            new() {
                Name = "Jake Holliday",
                School = "Michigan Valley University",
                Conference = "Big Ten",
                Position = "QB",
                DraftClassYear = 2025,
                HeightInches = 75, // 6'3"
                WeightLbs = 235,
                HomeTown = "Detroit",
                HomeState = "MI",
                JerseyNumber = 16,
                ProjectedRound = 2,
                ProjectedPick = 45,
                OverallGrade = 79.0,
                CeilingGrade = 84.0,
                FloorGrade = 70.0,
                NflComparison = "Kirk Cousins",
                Bio = "Veterano líder com cinco anos de starts universitários. Presença no vestiário e capacidade de gerenciar o jogo são seus maiores ativos. Não vai fazer você perder mas também não vai carregar um time.",
                SeasonStats = [
                    new() {
                        Year = 2022, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 348, Completions = 224, PassingYards = 2842, PassingTDs = 24,
                        Interceptions = 10, Sacks = 26, SackYardsLost = 168,
                        RushingAttempts = 52, RushingYards = 168, RushingTDs = 3,
                        CompletionPct = 64.4, YardsPerAttempt = 8.2, AdjYardsPerAttempt = 7.8,
                        PasserRating = 148.4, Qbr = 74.8, Epa = 84.2, EpaPerPlay = 0.20,
                        Cpoe = 1.8, PressureRate = 0.24, SackRate = 0.070, OnTargetThrowPct = 73.2,
                        ThirdDownConvPct = 42.8, RedZoneCompPct = 60.2, DropPct = 0.034
                    },
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 368, Completions = 240, PassingYards = 3120, PassingTDs = 27,
                        Interceptions = 11, Sacks = 24, SackYardsLost = 152,
                        RushingAttempts = 48, RushingYards = 148, RushingTDs = 2,
                        CompletionPct = 65.2, YardsPerAttempt = 8.5, AdjYardsPerAttempt = 8.1,
                        PasserRating = 152.2, Qbr = 77.8, Epa = 94.8, EpaPerPlay = 0.22,
                        Cpoe = 2.8, PressureRate = 0.22, SackRate = 0.061, OnTargetThrowPct = 74.8,
                        ThirdDownConvPct = 45.4, RedZoneCompPct = 62.8, DropPct = 0.031
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 382, Completions = 253, PassingYards = 3212, PassingTDs = 29,
                        Interceptions = 10, Sacks = 22, SackYardsLost = 140,
                        RushingAttempts = 44, RushingYards = 124, RushingTDs = 2,
                        CompletionPct = 66.2, YardsPerAttempt = 8.4, AdjYardsPerAttempt = 8.1,
                        PasserRating = 155.8, Qbr = 80.2, Epa = 102.4, EpaPerPlay = 0.23,
                        Cpoe = 3.2, PressureRate = 0.20, SackRate = 0.054, OnTargetThrowPct = 75.2,
                        ThirdDownConvPct = 47.2, RedZoneCompPct = 64.4, DropPct = 0.029
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 18),
                        ArmStrength = 80, Accuracy = 80, DeepBallAccuracy = 76, Footwork = 82,
                        Mechanics = 82, DecisionMaking = 85, PocketPresence = 83, MobilityAgility = 65,
                        Leadership = 92, FootballIq = 88, ReleaseSpeed = 76,
                        Strengths = "Liderança excepcional, confiável em situações de 2-minute drill, tomada de decisão madura, mecânica consistente",
                        Weaknesses = "Teto limitado, braço médio, mobilidade abaixo do padrão moderno da NFL",
                        NflComparison = "Kirk Cousins"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "NFL Combine", MeasuredAt = new DateTime(2025, 3, 2),
                        FortyYardDash = 4.78, TwentyYardSplit = 2.74, TenYardSplit = 1.65,
                        VerticalJump = 31.5, BroadJump = 104, ThreeConeDrill = 7.28,
                        ShuttleRun = 4.38, HandSizeInches = 10.0, ArmLengthInches = 33.25, WingSpanInches = 77.8
                    }
                ]
            },

            new() {
                Name = "Caleb Sanchez",
                School = "California West University",
                Conference = "Pac-12",
                Position = "QB",
                DraftClassYear = 2026,
                HeightInches = 76, // 6'4"
                WeightLbs = 225,
                HomeTown = "Los Angeles",
                HomeState = "CA",
                JerseyNumber = 2,
                ProjectedRound = 1,
                ProjectedPick = 15,
                OverallGrade = 68.0,
                CeilingGrade = 91.0,
                FloorGrade = 45.0,
                NflComparison = "Anthony Richardson (pré-draft)",
                Bio = "Espécimen físico com tamanho, braço e atletismo que enchem os olhos de qualquer scout. Porém, jogo ainda muito cru para um prospecto com dois anos de starts.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 9, GamesStarted = 7,
                        Attempts = 188, Completions = 108, PassingYards = 1580, PassingTDs = 16,
                        Interceptions = 10, Sacks = 26, SackYardsLost = 184,
                        RushingAttempts = 98, RushingYards = 548, RushingTDs = 7,
                        CompletionPct = 57.4, YardsPerAttempt = 8.4, AdjYardsPerAttempt = 7.6,
                        PasserRating = 132.4, Qbr = 62.8, Epa = 42.4, EpaPerPlay = 0.14,
                        Cpoe = -4.2, PressureRate = 0.36, SackRate = 0.122, OnTargetThrowPct = 63.8,
                        ThirdDownConvPct = 34.2, RedZoneCompPct = 48.8, DropPct = 0.058
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 298, Completions = 178, PassingYards = 2482, PassingTDs = 24,
                        Interceptions = 12, Sacks = 30, SackYardsLost = 212,
                        RushingAttempts = 128, RushingYards = 748, RushingTDs = 9,
                        CompletionPct = 59.7, YardsPerAttempt = 8.3, AdjYardsPerAttempt = 7.7,
                        PasserRating = 138.8, Qbr = 68.9, Epa = 62.8, EpaPerPlay = 0.16,
                        Cpoe = -2.8, PressureRate = 0.32, SackRate = 0.092, OnTargetThrowPct = 66.2,
                        ThirdDownConvPct = 36.8, RedZoneCompPct = 51.4, DropPct = 0.052
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 2, 20),
                        ArmStrength = 94, Accuracy = 65, DeepBallAccuracy = 72, Footwork = 70,
                        Mechanics = 65, DecisionMaking = 62, PocketPresence = 63, MobilityAgility = 91,
                        Leadership = 72, FootballIq = 65, ReleaseSpeed = 85,
                        Strengths = "Braço de elite, físico extraordinário, velocidade inesperada para o tamanho, projetis que poucos QB conseguem",
                        Weaknesses = "Pré-snap reads muito primitivos, leitura de cobertura superficial, sack rate alarmante, inconsistência crônica",
                        NflComparison = "Anthony Richardson (pré-draft)"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "Pro Day", MeasuredAt = new DateTime(2026, 3, 15),
                        FortyYardDash = 4.44, TwentyYardSplit = 2.57, TenYardSplit = 1.53,
                        VerticalJump = 42.5, BroadJump = 130, ThreeConeDrill = 6.74,
                        ShuttleRun = 4.04, HandSizeInches = 10.375, ArmLengthInches = 35.0, WingSpanInches = 82.1
                    }
                ]
            },

            new() {
                Name = "Devon Foster",
                School = "Penn Eastern University",
                Conference = "Big Ten",
                Position = "QB",
                DraftClassYear = 2025,
                HeightInches = 74, // 6'2"
                WeightLbs = 222,
                HomeTown = "Philadelphia",
                HomeState = "PA",
                JerseyNumber = 9,
                ProjectedRound = 2,
                ProjectedPick = 35,
                OverallGrade = 83.0,
                CeilingGrade = 88.0,
                FloorGrade = 72.0,
                NflComparison = "Matt Ryan",
                Bio = "Quarterback de estilo pro-style puro. Alto QI de futebol e leitura de field excepcional. Será um starter sólido na NFL mas não vai mudar o paradigma da liga.",
                SeasonStats = [
                    new() {
                        Year = 2022, Season = "Regular", GamesPlayed = 12, GamesStarted = 12,
                        Attempts = 348, Completions = 232, PassingYards = 2968, PassingTDs = 26,
                        Interceptions = 10, Sacks = 18, SackYardsLost = 114,
                        RushingAttempts = 38, RushingYards = 98, RushingTDs = 1,
                        CompletionPct = 66.7, YardsPerAttempt = 8.5, AdjYardsPerAttempt = 8.2,
                        PasserRating = 154.2, Qbr = 79.2, Epa = 96.4, EpaPerPlay = 0.22,
                        Cpoe = 3.4, PressureRate = 0.21, SackRate = 0.049, OnTargetThrowPct = 75.8,
                        ThirdDownConvPct = 47.2, RedZoneCompPct = 63.4, DropPct = 0.030
                    },
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 388, Completions = 264, PassingYards = 3348, PassingTDs = 30,
                        Interceptions = 10, Sacks = 16, SackYardsLost = 100,
                        RushingAttempts = 34, RushingYards = 88, RushingTDs = 1,
                        CompletionPct = 68.0, YardsPerAttempt = 8.6, AdjYardsPerAttempt = 8.4,
                        PasserRating = 158.8, Qbr = 82.4, Epa = 114.2, EpaPerPlay = 0.26,
                        Cpoe = 4.2, PressureRate = 0.19, SackRate = 0.040, OnTargetThrowPct = 77.4,
                        ThirdDownConvPct = 50.4, RedZoneCompPct = 67.2, DropPct = 0.027
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 408, Completions = 283, PassingYards = 3682, PassingTDs = 33,
                        Interceptions = 9, Sacks = 14, SackYardsLost = 88,
                        RushingAttempts = 32, RushingYards = 74, RushingTDs = 1,
                        CompletionPct = 69.4, YardsPerAttempt = 9.0, AdjYardsPerAttempt = 8.9,
                        PasserRating = 163.4, Qbr = 85.6, Epa = 128.8, EpaPerPlay = 0.30,
                        Cpoe = 5.2, PressureRate = 0.17, SackRate = 0.033, OnTargetThrowPct = 78.2,
                        ThirdDownConvPct = 51.8, RedZoneCompPct = 69.4, DropPct = 0.024
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 1, 25),
                        ArmStrength = 76, Accuracy = 87, DeepBallAccuracy = 82, Footwork = 84,
                        Mechanics = 85, DecisionMaking = 88, PocketPresence = 86, MobilityAgility = 62,
                        Leadership = 85, FootballIq = 91, ReleaseSpeed = 80,
                        Strengths = "QI de futebol de elite, manejo de progressões complexas, timing excelente, responsável com a bola",
                        Weaknesses = "Mobilidade muito limitada para padrões modernos, braço não elite em distância",
                        NflComparison = "Matt Ryan"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "NFL Combine", MeasuredAt = new DateTime(2025, 2, 26),
                        FortyYardDash = 4.82, TwentyYardSplit = 2.76, TenYardSplit = 1.67,
                        VerticalJump = 30.5, BroadJump = 101, ThreeConeDrill = 7.22,
                        ShuttleRun = 4.32, HandSizeInches = 9.75, ArmLengthInches = 32.5, WingSpanInches = 76.0
                    }
                ]
            },

            new() {
                Name = "Chase Anderson",
                School = "Georgia Southern University",
                Conference = "SEC",
                Position = "QB",
                DraftClassYear = 2026,
                HeightInches = 72, // 6'0"
                WeightLbs = 195,
                HomeTown = "Atlanta",
                HomeState = "GA",
                JerseyNumber = 4,
                ProjectedRound = 2,
                ProjectedPick = 52,
                OverallGrade = 66.0,
                CeilingGrade = 85.0,
                FloorGrade = 42.0,
                NflComparison = "Michael Vick (pré-draft)",
                Bio = "O mais emocionante prospecto para assistir da classe 2026. Eletrizante com os pés, mas a precisão e o nível de competição do SEC são preocupações reais.",
                SeasonStats = [
                    new() {
                        Year = 2023, Season = "Regular", GamesPlayed = 11, GamesStarted = 10,
                        Attempts = 228, Completions = 124, PassingYards = 1748, PassingTDs = 16,
                        Interceptions = 12, Sacks = 26, SackYardsLost = 184,
                        RushingAttempts = 148, RushingYards = 882, RushingTDs = 11,
                        CompletionPct = 54.4, YardsPerAttempt = 7.7, AdjYardsPerAttempt = 6.7,
                        PasserRating = 122.4, Qbr = 61.8, Epa = 38.4, EpaPerPlay = 0.11,
                        Cpoe = -6.8, PressureRate = 0.36, SackRate = 0.103, OnTargetThrowPct = 60.4,
                        ThirdDownConvPct = 32.4, RedZoneCompPct = 46.8, DropPct = 0.062
                    },
                    new() {
                        Year = 2024, Season = "Regular", GamesPlayed = 13, GamesStarted = 13,
                        Attempts = 268, Completions = 152, PassingYards = 2342, PassingTDs = 21,
                        Interceptions = 13, Sacks = 28, SackYardsLost = 198,
                        RushingAttempts = 168, RushingYards = 1048, RushingTDs = 12,
                        CompletionPct = 56.7, YardsPerAttempt = 8.7, AdjYardsPerAttempt = 7.8,
                        PasserRating = 128.4, Qbr = 67.2, Epa = 58.2, EpaPerPlay = 0.15,
                        Cpoe = -4.8, PressureRate = 0.32, SackRate = 0.095, OnTargetThrowPct = 62.8,
                        ThirdDownConvPct = 36.2, RedZoneCompPct = 50.4, DropPct = 0.058
                    }
                ],
                ScoutReports = [
                    new() {
                        Organization = "Prospector Analytics",
                        ReportDate = new DateTime(2025, 2, 18),
                        ArmStrength = 73, Accuracy = 58, DeepBallAccuracy = 64, Footwork = 65,
                        Mechanics = 62, DecisionMaking = 60, PocketPresence = 58, MobilityAgility = 98,
                        Leadership = 72, FootballIq = 63, ReleaseSpeed = 74,
                        Strengths = "Mobilidade transcendente que nenhum linebacker pode acompanhar, instinto para encontrar espaço, produção como corredor de elite",
                        Weaknesses = "Precisão abaixo do mínimo aceitável, interceptions em excesso, decisões precipitadas, mecânica precisará de reconstrução total",
                        NflComparison = "Michael Vick (pré-draft)"
                    }
                ],
                Measurements = [
                    new() {
                        Event = "Pro Day", MeasuredAt = new DateTime(2026, 3, 20),
                        FortyYardDash = 4.34, TwentyYardSplit = 2.50, TenYardSplit = 1.48,
                        VerticalJump = 43.5, BroadJump = 134, ThreeConeDrill = 6.58,
                        ShuttleRun = 3.94, HandSizeInches = 9.0, ArmLengthInches = 31.25, WingSpanInches = 73.2
                    }
                ]
            }
        };

        db.Players.AddRange(players);
        await db.SaveChangesAsync();
    }
}
