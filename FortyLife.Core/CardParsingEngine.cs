﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortyLife.Core
{
    public static class CardParsingEngine
    {
        // used for sorting colors from scryfall *eyeroll*
        public static readonly string WubrgOrder = "WUBRG";

        public static readonly Dictionary<string, string> WubrgHtmlColors = new Dictionary<string, string>
        {
            {"White", "#ffff99"},
            {"Blue", "#3366ff"},
            {"Black", "#3b373d"},
            {"Red", "#ff5050"},
            {"Green", "#69e569"},
            {"Colorless", "#6d6b72" } // #d2d7dd <-- old color backup
        };

        public static string FormatColor(string color)
        {
            return FormatColor(new List<string> { color });
        }

        public static string FormatColor(List<string> colors)
        {
            if (colors == null || colors.Count == 0)
                return "Colorless";

            var sb = new StringBuilder();
            var sortedColors = colors.OrderBy(i => WubrgOrder.IndexOf(i, StringComparison.Ordinal));

            foreach (var color in sortedColors)
            {
                switch (color)
                {
                    case "W":
                        sb.Append("White ");
                        break;

                    case "U":
                        sb.Append("Blue ");
                        break;

                    case "B":
                        sb.Append("Black ");
                        break;

                    case "R":
                        sb.Append("Red ");
                        break;

                    case "G":
                        sb.Append("Green ");
                        break;
                }
            }

            return sb.ToString().Remove(sb.Length - 1);
        }

        public static string RenderManaSymbols(string originalText)
        {
            if (originalText == null)
                return string.Empty;

            // Color symbols
            var newText = originalText.Replace("{W}", "<i class=\"ms ms-w ms-cost ms-shadow\" title=\"one white mana\"></i>");
            newText = newText.Replace("{U}", "<i class=\"ms ms-u ms-cost ms-shadow\" title=\"one blue mana\"></i>");
            newText = newText.Replace("{B}", "<i class=\"ms ms-b ms-cost ms-shadow\" title=\"one black mana\"></i>");
            newText = newText.Replace("{R}", "<i class=\"ms ms-r ms-cost ms-shadow\" title=\"one red mana\"></i>");
            newText = newText.Replace("{G}", "<i class=\"ms ms-g ms-cost ms-shadow\" title=\"one green mana\"></i>");
            newText = newText.Replace("{C}", "<i class=\"ms ms-c ms-cost ms-shadow\" title=\"one colorless mana\"></i>");

            // Zero mana symbol
            newText = newText.Replace("{0}", "<i class=\"ms ms-0 ms-cost ms-shadow\" title=\"zero mana\"></i>");

            // Generic symbols
            newText = newText.Replace("{X}", "<i class=\"ms ms-x ms-cost ms-shadow\" title=\"X generic mana\"></i>");

            for (var i = 1; i <= 20; i++)
            {
                newText = newText.Replace($"{{{i}}}", $"<i class=\"ms ms-{i} ms-cost ms-shadow\" title=\"{(i == 1 ? "one" : i.ToString())} generic mana\"></i>");
            }

            // Split symbols
            newText = newText.Replace("{W/U}", "<i class=\"ms ms-wu ms-cost ms-shadow\" title=\"one white or blue mana\"></i>");
            newText = newText.Replace("{W/B}", "<i class=\"ms ms-wb ms-cost ms-shadow\" title=\"one white or black mana\"></i>");
            newText = newText.Replace("{B/R}", "<i class=\"ms ms-br ms-cost ms-shadow\" title=\"one black or red mana\"></i>");
            newText = newText.Replace("{B/G}", "<i class=\"ms ms-bg ms-cost ms-shadow\" title=\"one black or green mana\"></i>");
            newText = newText.Replace("{U/B}", "<i class=\"ms ms-ub ms-cost ms-shadow\" title=\"one blue or black mana\"></i>");
            newText = newText.Replace("{U/R}", "<i class=\"ms ms-ur ms-cost ms-shadow\" title=\"one blue or red mana\"></i>");
            newText = newText.Replace("{R/G}", "<i class=\"ms ms-rg ms-cost ms-shadow\" title=\"one red or green mana\"></i>");
            newText = newText.Replace("{R/W}", "<i class=\"ms ms-rw ms-cost ms-shadow\" title=\"one red or white mana\"></i>");
            newText = newText.Replace("{G/W}", "<i class=\"ms ms-gw ms-cost ms-shadow\" title=\"one green or white mana\"></i>");
            newText = newText.Replace("{G/U}", "<i class=\"ms ms-gu ms-cost ms-shadow\" title=\"one green or blue mana\"></i>");
            newText = newText.Replace("{2/W}", "<i class=\"ms ms-2w ms-cost ms-shadow\" title=\"one white or 2 generic mana\"></i>");
            newText = newText.Replace("{2/U}", "<i class=\"ms ms-2u ms-cost ms-shadow\" title=\"one blue or 2 generic mana\"></i>");
            newText = newText.Replace("{2/B}", "<i class=\"ms ms-2b ms-cost ms-shadow\" title=\"one black or 2 generic mana\"></i>");
            newText = newText.Replace("{2/R}", "<i class=\"ms ms-2r ms-cost ms-shadow\" title=\"one red or 2 generic mana\"></i>");
            newText = newText.Replace("{2/G}", "<i class=\"ms ms-2g ms-cost ms-shadow\" title=\"one green or 2 generic mana\"></i>");

            // Phyrexian symbols
            newText = newText.Replace("{W/P}", "<i class=\"ms ms-wp ms-cost ms-shadow\" title=\"one white mana or 2 life\"></i>");
            newText = newText.Replace("{U/P}", "<i class=\"ms ms-up ms-cost ms-shadow\" title=\"one blue mana or 2 life\"></i>");
            newText = newText.Replace("{B/P}", "<i class=\"ms ms-bp ms-cost ms-shadow\" title=\"one black mana or 2 life\"></i>");
            newText = newText.Replace("{R/P}", "<i class=\"ms ms-rp ms-cost ms-shadow\" title=\"one red mana or 2 life\"></i>");
            newText = newText.Replace("{G/P}", "<i class=\"ms ms-gp ms-cost ms-shadow\" title=\"one green mana or 2 life\"></i>");

            // Un- symbols
            newText = newText.Replace("{HW}", "<i class=\"ms ms-w ms-half ms-cost ms-shadow\" title=\"one-half white mana\"></i>");
            newText = newText.Replace("{HR}", "<i class=\"ms ms-r ms-half ms-cost ms-shadow\" title=\"one-half red mana\"></i>");

            newText = newText.Replace("{Y}", "<i class=\"ms ms-y ms-cost ms-shadow\" title=\"Y generic mana\"></i>");
            newText = newText.Replace("{Z}", "<i class=\"ms ms-z ms-cost ms-shadow\" title=\"Z generic mana\"></i>");

            newText = newText.Replace("{1/2}", "<i class=\"ms ms-1-2 ms-cost ms-shadow\" title=\"one-half generic mana\"></i>");
            newText = newText.Replace("{100}", "<i class=\"ms ms-100 ms-cost ms-shadow\" title=\"one hundred generic mana\"></i>");
            newText = newText.Replace("{1000000}", "<i class=\"ms ms-c ms-cost ms-shadow\" title=\"one million generic mana\"></i>");
            newText = newText.Replace("{∞}", "<i class=\"ms ms-infinity ms-cost ms-shadow\" title=\"infinite generic mana\"></i>");

            return newText;
        }

        public static string RenderSymbols(string originalText)
        {
            if (originalText == null)
                return string.Empty;

            var newText = originalText.Replace("{T}", "<i class=\"ms ms-tap-alt ms-cost ms-shadow\" title=\"tap this permanent\"></i>"); // tap
            newText = newText.Replace("{U}", "<i class=\"ms ms-untap ms-cost ms-shadow\" title=\"untap this permanent\"></i>"); // untap
            newText = newText.Replace("{E}", "<i class=\"ms ms-e\" title=\"an energy counter\"></i>"); // energy
            newText = newText.Replace("{P}", "<i class=\"ms ms-p\" title=\"one colored mana or 2 life\"></i>"); // phyrexian color
            newText = newText.Replace("{S}", "<i class=\"ms ms-s ms-cost ms-shadow\" title=\"one snow mana\"></i>"); // snow

            newText = RenderManaSymbols(newText);

            return newText;
        }

        public static string RenderSetSymbol(string setShortCode, string rarity)
        {
            return $"<i class=\"ss ss-{setShortCode} ss-{rarity}\" title=\"{rarity}\"></i>";
        }

        public static string RenderLineBreaks(string cardText)
        {
            return cardText?.Replace("\n", "<br />");
        }

        public static string RemoveReminderText(string cardText)
        {
            var startIndex = 0;
            var endIndex = 0;

            for (var i = 0; i < cardText.Length; i++)
            {
                if (cardText[i] == '(')
                {
                    startIndex = i;
                }

                if (cardText[i] == ')')
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex > 0 && endIndex > 0)
            {
                return cardText.Remove(startIndex, endIndex - startIndex + 1);
            }

            return cardText;
        }

        public static string RenderCardType(string typeLine)
        {
            var typeText = typeLine.Split('—')[0].Trim();
            var superTypes = typeText.ReplaceSupertypes().ToLower().Trim().Split(' ');
            var typeIcon = $"<i class=\"ms ms-{superTypes[0]}\" title=\"{superTypes[0]}\"></i> ";

            if (superTypes.Length > 1)
            {
                typeIcon += $"<i class=\"ms ms-{superTypes[1]}\" title=\"{superTypes[1]}\"></i> ";
            }
            
            return typeIcon + typeText;
        }

        public static string RenderColorWheel(List<string> colors)
        {
            var pieHtml = $"<div class=\"d-flex justify-content-center\"> <div class=\"pie pie-icon-size\" title=\"{FormatColor(colors)}\">\r\n";
            var offset = 0;
            int step;

            if (colors == null || colors.Count == 0)
            {
                // Each slice can only represent 50% of a wheel, so render the other half
                pieHtml += $"<div class=\"pie__segment\" style=\"--offset: 0; --value: 50; --bg: {WubrgHtmlColors["Colorless"]}\"></div>\r\n";
                pieHtml += $"<div class=\"pie__segment\" style=\"--offset: 50; --value: 50; --bg: {WubrgHtmlColors["Colorless"]}\"></div>\r\n";
                pieHtml += "</div></div>\r\n";

                return pieHtml;
            }

            var sortedColors = colors.OrderBy(i => WubrgOrder.IndexOf(i, StringComparison.Ordinal)).ToList();

            switch (sortedColors.Count)
            {
                default: // Each slice can only represent 50% of a wheel, so no need for cases 1 or 2 (special handling in place for case 1)
                    step = 50;
                    break;

                case 3:
                    step = 33;
                    break;

                case 4:
                    step = 25;
                    break;

                case 5:
                    step = 20;
                    break;
            }

            foreach (var color in sortedColors)
            {
                pieHtml += $"<div class=\"pie__segment\" style=\"--offset: {offset}; --value: {step}; --bg: {WubrgHtmlColors[FormatColor(color)]}\"></div>\r\n";
                offset += step;

                // The only case where the parts don't add up to 100 (100 / 3 = stupid rounding)...
                // so artifically make the last segment 34 to fill in the pie and kill that dead 1 pixel length line
                if (colors.Count == 3 && offset == 66)
                {
                    step++;
                }

                // Each slice can only represent 50% of a wheel, so render the other half
                if (colors.Count == 1)
                {
                    pieHtml +=
                        $"<div class=\"pie__segment\" style=\"--offset: {offset}; --value: {step}; --bg: {WubrgHtmlColors[FormatColor(color)]}\"></div>\r\n";
                }
            }

            pieHtml += "</div></div>\r\n";

            return pieHtml;
        }

        public static string ReplaceSupertypes(this string type)
        {
            return type.Replace("Legendary", string.Empty).Replace("Basic", string.Empty).Replace("Snow", string.Empty)
                .Replace("World", string.Empty).Replace("Host", string.Empty);
        }
    }
}
