using System;
using System.Collections.Generic;
using System.Linq;
using AnimeNowApi.Models;

namespace AnimeNowApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AnimeDbContext context)
        {
            context.Database.EnsureCreated();

            // 檢查是否已有數據
            if (context.Bangumis.Any())
            {
                return;
            }

            // 添加類型
            var genres = new Genre[]
            {
                new Genre { Name = "奇幻" },
                new Genre { Name = "冒險" },
                new Genre { Name = "治癒" },
                new Genre { Name = "懸疑" },
                new Genre { Name = "音樂" },
                new Genre { Name = "偶像" },
                new Genre { Name = "異世界" },
                new Genre { Name = "校園" },
                new Genre { Name = "青春" }
            };

            foreach (Genre g in genres)
            {
                context.Genres.Add(g);
            }
            context.SaveChanges();

            // 添加番劇
            var bangumis = new Bangumi[]
            {
                new Bangumi
                {
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
                new Bangumi
                {
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
                new Bangumi
                {
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
                new Bangumi
                {
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

            foreach (Bangumi b in bangumis)
            {
                context.Bangumis.Add(b);
            }
            context.SaveChanges();

            // 取得所有已保存的番劇和類型，便後續使用
            var bangumisDict = context.Bangumis.ToDictionary(b => b.Id);
            var genresDict = context.Genres.ToDictionary(g => g.Id);

            // 添加番劇與類型關聯
            var bangumiGenres = new BangumiGenre[]
            {
                new BangumiGenre {
                    BangumiId = 1,
                    GenreId = 1,
                    Bangumi = bangumisDict[1],
                    Genre = genresDict[1]
                },
                new BangumiGenre {
                    BangumiId = 1,
                    GenreId = 2,
                    Bangumi = bangumisDict[1],
                    Genre = genresDict[2]
                },
                new BangumiGenre {
                    BangumiId = 1,
                    GenreId = 3,
                    Bangumi = bangumisDict[1],
                    Genre = genresDict[3]
                },
                new BangumiGenre {
                    BangumiId = 2,
                    GenreId = 4,
                    Bangumi = bangumisDict[2],
                    Genre = genresDict[4]
                },
                new BangumiGenre {
                    BangumiId = 2,
                    GenreId = 5,
                    Bangumi = bangumisDict[2],
                    Genre = genresDict[5]
                },
                new BangumiGenre {
                    BangumiId = 2,
                    GenreId = 6,
                    Bangumi = bangumisDict[2],
                    Genre = genresDict[6]
                },
                new BangumiGenre {
                    BangumiId = 3,
                    GenreId = 1,
                    Bangumi = bangumisDict[3],
                    Genre = genresDict[1]
                },
                new BangumiGenre {
                    BangumiId = 3,
                    GenreId = 2,
                    Bangumi = bangumisDict[3],
                    Genre = genresDict[2]
                },
                new BangumiGenre {
                    BangumiId = 3,
                    GenreId = 7,
                    Bangumi = bangumisDict[3],
                    Genre = genresDict[7]
                },
                new BangumiGenre {
                    BangumiId = 4,
                    GenreId = 5,
                    Bangumi = bangumisDict[4],
                    Genre = genresDict[5]
                },
                new BangumiGenre {
                    BangumiId = 4,
                    GenreId = 8,
                    Bangumi = bangumisDict[4],
                    Genre = genresDict[8]
                },
                new BangumiGenre {
                    BangumiId = 4,
                    GenreId = 9,
                    Bangumi = bangumisDict[4],
                    Genre = genresDict[9]
                }
            };

            foreach (BangumiGenre bg in bangumiGenres)
            {
                context.BangumiGenres.Add(bg);
            }
            context.SaveChanges();

            // 添加劇集
            var episodes = new Episode[]
            {
                new Episode
                {
                    BangumiId = 1,
                    Number = 1,
                    Title = "精靈的旅程",
                    AirDate = new DateTime(2025, 4, 5),
                    Duration = "24:30",
                    Description = "魔王被打倒後，精靈魔法使芙莉蓮踏上尋找人類情感的旅途。",
                    Views = 15420,
                    Bangumi = bangumisDict[1]
                },
                new Episode
                {
                    BangumiId = 1,
                    Number = 2,
                    Title = "回憶的重量",
                    AirDate = new DateTime(2025, 4, 12),
                    Duration = "24:15",
                    Description = "芙莉蓮回憶起與昔日夥伴的冒險，決定尋找新的同伴。",
                    Views = 12830,
                    Bangumi = bangumisDict[1]
                },
                new Episode
                {
                    BangumiId = 1,
                    Number = 3,
                    Title = "魔法的真諦",
                    AirDate = new DateTime(2025, 4, 19),
                    Duration = "24:45",
                    Description = "芙莉蓮收了新弟子，開始教導她了解魔法的本質。",
                    Views = 10240,
                    Bangumi = bangumisDict[1]
                },
                new Episode
                {
                    BangumiId = 2,
                    Number = 1,
                    Title = "偶像的真實",
                    AirDate = new DateTime(2025, 4, 6),
                    Duration = "23:40",
                    Description = "偶像組合 B 小隊面臨解散危機，愛開始懷疑自己的選擇。",
                    Views = 18720,
                    Bangumi = bangumisDict[2]
                },
                new Episode
                {
                    BangumiId = 2,
                    Number = 2,
                    Title = "命運的交錯",
                    AirDate = new DateTime(2025, 4, 13),
                    Duration = "23:50",
                    Description = "新角色的加入打亂了原有的平衡，背後似乎隱藏著不為人知的秘密。",
                    Views = 16540,
                    Bangumi = bangumisDict[2]
                }
            };

            foreach (Episode e in episodes)
            {
                context.Episodes.Add(e);
            }
            context.SaveChanges();
        }
    }
}