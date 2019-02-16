﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using FortyLife.DataAccess.Scryfall;
using Newtonsoft.Json;

namespace FortyLife.DataAccess
{
    public class ScryfallRequestEngine : RequestEngine
    {
        private const string BaseSearchUri = "https://api.scryfall.com/cards/search";
        private const string SetSearchUri = "https://scryfall.com/set/";

        private T Request<T>(string requestUri) where T : new()
        {
            // https://scryfall.com/docs/api#type-error (see: "Rate Limits and Good Citizenship")
            // try to delay the request time by 200 ms, so that in perfect sequence we can only hope to pull off 5 requests per second
            // scryfall will ban this IP if their endpoints are abused and they would like us to limit our requests to 10 per second, anyway
            Thread.Sleep(200); // TODO: better way to rate limit without shutting the thread down entirely
            // TODO: handle the 429 status code (if we ever even get it back) from scryfall

            var jsonResult = Get(requestUri).Replace("_", string.Empty);

            return !string.IsNullOrEmpty(jsonResult) ? JsonConvert.DeserializeObject<T>(jsonResult) : new T();
        }

        public ScryfallList<Card> CardSearchRequest(string cardName)
        {
            var request = Request<ScryfallList<Card>>($"{BaseSearchUri}?q=name={cardName}");

            if (request.Data != null)
            {
                for (var i = 0; i < request.Data.Count; i++)
                {
                    if (request.Data[i].Digital)
                    {
                        request.Data[i] = FirstCardFromSearch(request.Data[i].Name);
                    }
                }
            }

            return request;
        }

        public Card FirstCardFromSearch(string cardName, string setCode = "")
        {
            using (var db = new FortyLifeDbContext())
            {
                if (!string.IsNullOrEmpty(setCode))
                {
                    if (db.Cards.Any(i => i.Name == cardName && i.Set == setCode && DbFunctions.DiffDays(i.CacheDate, DateTime.Now) < 7))
                    {
                        return db.Cards.FirstOrDefault(i => i.Name == cardName && i.Set == setCode);
                    }
                }
                else
                {
                    if (db.Cards.Any(i => i.Name == cardName && DbFunctions.DiffDays(i.CacheDate, DateTime.Now) < 7))
                    {
                        return db.Cards.FirstOrDefault(i => i.Name == cardName);
                    }
                }

                var searchResultList = CardPrintingsRequest(cardName);
                var card = !string.IsNullOrEmpty(setCode)
                    ? searchResultList.Data?.FirstOrDefault(i =>
                        i.Name == cardName &&
                        string.Equals(i.Set, setCode, StringComparison.CurrentCultureIgnoreCase))
                    : searchResultList.Data?.FirstOrDefault(i => i.Name == cardName);

                if (card != null)
                {
                    card.CacheDate = DateTime.Now;
                    db.Cards.AddOrUpdate(card);
                    db.SaveChanges();
                }

                return card;
            }
        }

        public ScryfallList<Card> CardPrintingsRequest(string cardName)
        {
            var results = Request<ScryfallList<Card>>($"{BaseSearchUri}?q=name={cardName}&unique=prints");
            results.Data = results.Data.Where(i => i.Digital == false && i.Name == cardName).ToList();

            return results;
        }

        public List<SetName> CardPrintingsSetNames(string cardName)
        {
            var setNames = new List<SetName>();

            foreach (var printing in CardPrintingsRequest(cardName).Data)
            {
                setNames.Add(new SetName
                {
                    Code = printing.Set,
                    Name = printing.SetName,
                    Rarity = printing.Rarity
                });
            }

            return setNames;
        }

        public Set CardSetRequest(string setUri)
        {
            return Request<Set>(setUri);
        }

        public int SetCardCount(string setUri)
        {
            return CardSetRequest(setUri).CardCount;
        }

        public ScryfallList<Ruling> RulingsRequest(string rulingsUri)
        {
            return Request<ScryfallList<Ruling>>(rulingsUri);
        }
    }
}
