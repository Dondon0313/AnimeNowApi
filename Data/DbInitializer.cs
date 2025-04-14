using System;
using System.Collections.Generic;
using System.Linq;
using AnimeNowApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeNowApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                // 確保資料庫存在
                context.Database.EnsureCreated();

                logger?.LogInformation("正在檢查資料庫結構...");

                // 確認資料表結構
                EnsureDatabaseStructure(context, logger);

                // 檢查資料庫中是否已有資料
                if (context.Bangumis.Any())
                {
                    logger?.LogInformation("資料庫已包含番劇資料，跳過初始化");
                    return;
                }

                logger?.LogInformation("開始從外部資料來源載入資料...");

                // 實際環境中，可以從外部資料來源載入資料，從 API、CSV 檔案或其他資料庫

                // 載入類型資料
                LoadGenres(context, logger);

                // 載入番劇資料
                LoadBangumis(context, logger);

                // 載入番劇與類型的關聯
                LoadBangumiGenres(context, logger);

                // 載入劇集資料
                LoadEpisodes(context, logger);

                logger?.LogInformation("資料初始化完成");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "初始化資料庫時發生錯誤");
                throw;
            }
        }

        private static void EnsureDatabaseStructure(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                // 顯式刪除並重新創建資料庫
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                logger?.LogInformation("資料庫結構已重新創建");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "重新創建資料庫結構時發生錯誤");
            }
        }

        private static void LoadGenres(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                // 在實際環境中，可以從外部資料來源讀取類型資料，從 API 或 JSON 檔案
                // 先簡化，使用靜態資料，之後再改

                logger?.LogInformation("載入類型資料...");

                var genreNames = new[]
                {
                    "奇幻", "冒險", "治癒", "懸疑", "音樂",
                    "偶像", "異世界", "校園", "青春"
                };

                foreach (var name in genreNames)
                {
                    if (!context.Genres.Any(g => g.Name == name))
                    {
                        context.Genres.Add(new Genre { Name = name });
                    }
                }

                context.SaveChanges();
                logger?.LogInformation($"成功載入 {genreNames.Length} 個類型");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "載入類型資料時發生錯誤");
                throw;
            }
        }

        private static void LoadBangumis(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                // 在實際環境中，可以從外部資料來源讀取番劇資料，先使用靜態資料

                logger?.LogInformation("載入番劇資料...");

                var bangumiData = new[]
                {
                    new {
                        Title = "葬送的芙莉蓮",
                        Image = "https://cdn.myanimelist.net/images/anime/1015/138006.jpg",
                        AirDate = new DateTime(2025, 4, 5),
                        WeekDay = "星期六",
                        Description = "魔王被打倒後，精靈魔法使芙莉蓮踏上尋找人類情感的旅途。",
                        TotalEpisodes = 12,
                        Rating = 9.2m,
                        Studio = "範例工作室",
                        Status = "ongoing"
                    },
                    new {
                        Title = "我推的孩子 第二季",
                        Image = "/images/我推.jpg",
                        AirDate = new DateTime(2025, 4, 6),
                        WeekDay = "星期日",
                        Description = "偶像與轉生的交錯命運，再次揭開娛樂圈的真相。",
                        TotalEpisodes = 13,
                        Rating = 9.0m,
                        Studio = "B站動畫",
                        Status = "ongoing"
                    },
                    new {
                        Title = "無職轉生 第二季",
                        Image = "/images/無職.jpg",
                        AirDate = new DateTime(2025, 4, 7),
                        WeekDay = "星期一",
                        Description = "重生的魯迪烏斯展開全新冒險與成長，走出過去陰影。",
                        TotalEpisodes = 12,
                        Rating = 8.9m,
                        Studio = "MAPPA",
                        Status = "ongoing"
                    },
                    new {
                        Title = "吹響吧！上低音號 第三季",
                        Image = "/images/低音號.jpg",
                        AirDate = new DateTime(2025, 4, 8),
                        WeekDay = "星期二",
                        Description = "久美子與北宇治高中吹奏部邁向全國大賽的最後挑戰。",
                        TotalEpisodes = 13,
                        Rating = 8.7m,
                        Studio = "京都動畫",
                        Status = "ongoing"
                    }
                };

                foreach (var data in bangumiData)
                {
                    if (!context.Bangumis.Any(b => b.Title == data.Title))
                    {
                        context.Bangumis.Add(new Bangumi
                        {
                            Title = data.Title,
                            Image = data.Image,
                            AirDate = data.AirDate,
                            WeekDay = data.WeekDay,
                            Description = data.Description,
                            TotalEpisodes = data.TotalEpisodes,
                            Rating = data.Rating,
                            Studio = data.Studio,
                            Status = data.Status
                        });
                    }
                }

                context.SaveChanges();
                logger?.LogInformation($"成功載入 {bangumiData.Length} 個番劇");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "載入番劇資料時發生錯誤");
                throw;
            }
        }

        private static void LoadBangumiGenres(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                logger?.LogInformation("載入番劇與類型關聯...");

                // 獲取所有已儲存的番劇和類型
                var bangumis = context.Bangumis.ToList();
                var genres = context.Genres.ToList();

                // 定義關聯數據
                var associations = new[]
                {
                    new { BangumiTitle = "葬送的芙莉蓮", GenreNames = new[] { "奇幻", "冒險", "治癒" } },
                    new { BangumiTitle = "我推的孩子 第二季", GenreNames = new[] { "懸疑", "音樂", "偶像" } },
                    new { BangumiTitle = "無職轉生 第二季", GenreNames = new[] { "奇幻", "冒險", "異世界" } },
                    new { BangumiTitle = "吹響吧！上低音號 第三季", GenreNames = new[] { "音樂", "校園", "青春" } }
                };

                foreach (var assoc in associations)
                {
                    var bangumi = bangumis.FirstOrDefault(b => b.Title == assoc.BangumiTitle);
                    if (bangumi == null) continue;

                    foreach (var genreName in assoc.GenreNames)
                    {
                        var genre = genres.FirstOrDefault(g => g.Name == genreName);
                        if (genre == null) continue;

                        // 檢查關聯是否已存在
                        if (!context.BangumiGenres.Any(bg =>
                            bg.BangumiId == bangumi.Id && bg.GenreId == genre.Id))
                        {
                            context.BangumiGenres.Add(new BangumiGenre
                            {
                                BangumiId = bangumi.Id,
                                GenreId = genre.Id,
                                Bangumi = bangumi,
                                Genre = genre
                            });
                        }
                    }
                }

                context.SaveChanges();
                logger?.LogInformation("成功載入番劇與類型關聯");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "載入番劇與類型關聯時發生錯誤");
                throw;
            }
        }

        private static void LoadEpisodes(AnimeDbContext context, ILogger? logger = null)
        {
            try
            {
                logger?.LogInformation("載入劇集資料...");

                // 獲取所有已儲存的番劇
                var bangumis = context.Bangumis.ToDictionary(b => b.Title);

                // 定義劇集資料
                var episodeData = new[]
                {
                    new {
                        BangumiTitle = "葬送的芙莉蓮",
                        Number = 1,
                        Title = "精靈的旅程",
                        AirDate = new DateTime(2025, 4, 5),
                        Duration = "24:30",
                        Description = "魔王被打倒後，精靈魔法使芙莉蓮踏上尋找人類情感的旅途。",
                        Views = 15420
                    },
                    new {
                        BangumiTitle = "葬送的芙莉蓮",
                        Number = 2,
                        Title = "回憶的重量",
                        AirDate = new DateTime(2025, 4, 12),
                        Duration = "24:15",
                        Description = "芙莉蓮回憶起與昔日夥伴的冒險，決定尋找新的同伴。",
                        Views = 12830
                    },
                    new {
                        BangumiTitle = "葬送的芙莉蓮",
                        Number = 3,
                        Title = "魔法的真諦",
                        AirDate = new DateTime(2025, 4, 19),
                        Duration = "24:45",
                        Description = "芙莉蓮收了新弟子，開始教導她了解魔法的本質。",
                        Views = 10240
                    },
                    new {
                        BangumiTitle = "我推的孩子 第二季",
                        Number = 1,
                        Title = "偶像的真實",
                        AirDate = new DateTime(2025, 4, 6),
                        Duration = "23:40",
                        Description = "偶像組合 B 小隊面臨解散危機，愛開始懷疑自己的選擇。",
                        Views = 18720
                    },
                    new {
                        BangumiTitle = "我推的孩子 第二季",
                        Number = 2,
                        Title = "命運的交錯",
                        AirDate = new DateTime(2025, 4, 13),
                        Duration = "23:50",
                        Description = "新角色的加入打亂了原有的平衡，背後似乎隱藏著不為人知的秘密。",
                        Views = 16540
                    }
                };

                foreach (var data in episodeData)
                {
                    if (!bangumis.ContainsKey(data.BangumiTitle)) continue;

                    var bangumi = bangumis[data.BangumiTitle];

                    // 檢查劇集是否已存在
                    if (!context.Episodes.Any(e =>
                        e.BangumiId == bangumi.Id && e.Number == data.Number))
                    {
                        context.Episodes.Add(new Episode
                        {
                            BangumiId = bangumi.Id,
                            Number = data.Number,
                            Title = data.Title,
                            AirDate = data.AirDate,
                            Duration = data.Duration,
                            Description = data.Description,
                            Views = data.Views,
                            Bangumi = bangumi
                        });
                    }
                }

                context.SaveChanges();
                logger?.LogInformation($"成功載入 {episodeData.Length} 個劇集");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "載入劇集資料時發生錯誤");
                throw;
            }
        }
    }
}