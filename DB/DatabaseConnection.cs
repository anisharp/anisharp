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
        public void addEntry(serie newSeries)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                context.serie.AddObject(newSeries);
                context.SaveChanges();
            }
        }

        public void addEntry(groups newGroup)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                context.groups.AddObject(newGroup);
                context.SaveChanges();
            }
        }

        public void addEntry(episode newEpisode)
        {
            using (AniSharpDBEntities context = new AniSharpDBEntities())
            {
                context.episode.AddObject(newEpisode);
                context.SaveChanges();
            }
        }

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
            }
        }

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
    }
}
