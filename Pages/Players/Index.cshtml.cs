using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using League.Data;
using League.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace League.Pages.Players
{
    public class IndexModel : PageModel
    {
        private readonly LeagueContext _context;
        public List<Player> Players { get; set; }
        public SelectList Teams { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedTeam { get; set; }
        public SelectList Positions { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedPosition { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; } = "Name";
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public string FavoriteTeam { get; set; }

        public IndexModel(LeagueContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            IQueryable<Player> players = from p in _context.Players
                                         select p;
            if (!string.IsNullOrEmpty(SelectedTeam))
            {
                players.Where(p => p.TeamId == SelectedTeam);
            }
            if (!string.IsNullOrEmpty(SelectedPosition))
            {
                players.Where(p => p.Position == SelectedPosition);
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                players.Where(p => SearchString.Contains(p.Name));
            }
            switch (SortField)
            {
                case "Number": players = players.OrderBy(p => p.Number).ThenBy(p => p.TeamId); break;
                case "Name": players = players.OrderBy(p => p.Name).ThenBy(p => p.TeamId); break;
                case "Position": players = players.OrderBy(p => p.Position).ThenBy(p => p.TeamId); break;
            }
            Players = await players.ToListAsync();

            IQueryable<string> teamsQuery = from t in _context.Teams
                                            orderby t.TeamId
                                            select t.TeamId;
            Teams = new SelectList(await teamsQuery.ToListAsync());

            IQueryable<string> positionQuery = from p in _context.Players
                                               orderby p.Position
                                               select p.Position;
            Positions = new SelectList(await positionQuery.Distinct().ToListAsync());

            FavoriteTeam = HttpContext.Session.GetString("_Favorite");

        }

        public string PlayerClass(Player Player)
        {
            string Class = "d-flex";
            if (Player.Depth == 1)
                Class += " starter";
            if (Player.TeamId == FavoriteTeam)
                Class += " favorite";
            return Class;
        }
    }
}
