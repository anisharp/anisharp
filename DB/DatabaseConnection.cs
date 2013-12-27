using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using System.Data.EntityClient;

namespace AniSharp
{
    class DatabaseConnection
    {
        /// <summary>
        /// Adds new anime to db
        /// </summary>
        /// <param name="newSeries">Anime to add</param>
        public void addEntry(serie newSeries)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                context.serie.AddObject(newSeries);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds new group to db
        /// </summary>
        /// <param name="newGroup">Group to add</param>
        public void addEntry(groups newGroup)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                context.groups.AddObject(newGroup);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds new episode to db
        /// </summary>
        /// <param name="newEpisode">Episode to add</param>
        public void addEntry(episode newEpisode)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                try
                {
                    context.episode.AddObject(newEpisode);
                    context.SaveChanges();
                }
                catch (UpdateException e)
                {
                }
            }
        }

        /// <summary>
        /// Gets episode from db
        /// </summary>
        /// <param name="hash">Hash of file</param>
        /// <returns>Episode from db</returns>
        public episode getEpisode(string hash)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                var episodes = (from o in context.episode where o.ed2k.Equals(hash) select o);
                try
                {
                    return episodes.First();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                
            }
        }

        /// <summary>
        /// Gets anime from db
        /// </summary>
        /// <param name="id">Id of anime</param>
        /// <returns>Anime from db</returns>
        public serie getSeries(int id)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                var series = (from o in context.serie where o.serienId.Equals(id) select o);
                try
                {
                    return series.First();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets group from db
        /// </summary>
        /// <param name="id">Id of group</param>
        /// <returns>Group from db</returns>
        public groups getGroup(int id)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                var group = (from o in context.groups where o.groupsId.Equals(id) select o);
                try
                {
                    return group.First();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        public void updateEntry(serie updateSerie)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                try
                {
                    serie s= context.serie.First(i => i.serienId == updateSerie.serienId);
                    //context.serie.AddObject(updateSerie);
                    s.category = updateSerie.category;
                    s.englishName = updateSerie.englishName;
                    s.highestNoEp = updateSerie.highestNoEp;
                    s.kanjiName = updateSerie.kanjiName;
                    s.otherName= updateSerie.otherName;
                    s.rating= updateSerie.rating;
                    s.romajiName = updateSerie.romajiName;
                    s.shortName = updateSerie.shortName;
                    s.specialEpCount = updateSerie.specialEpCount;
                    s.tempRating = updateSerie.tempRating;
                    s.type = updateSerie.type;
                    context.SaveChanges();
                }
                catch (UpdateException e)
                {
                }
            }
        }
    }
}
