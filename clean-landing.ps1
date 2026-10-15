Add-Type -TypeDefinition @'
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class LandingCleaner {
    public static void Run(string path) {
        string html = File.ReadAllText(path, Encoding.UTF8);

        // ── 1. REPLACE everything from the opening comment + <head> through </head> ──
        string newHead =
@"<head>
	<meta charset=""UTF-8"">
	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0, user-scalable=no"">
	<title>Return Trip | Cargo &amp; Logistics</title>

	<!-- Theme CSS bundle (Motors theme + plugins) -->
	<link rel=""stylesheet"" href=""wp-content/cache/autoptimize/5/css/autoptimize_8563778c5fa279d1cdca725bb17a00b7.css"">

	<!-- Theme icon font -->
	<link rel=""stylesheet"" href=""wp-content/cache/autoptimize/5/css/autoptimize_single_56429eba7b6d376709d6c61a53834af2a758.css?ver=7.0.7"">

	<!-- Google Fonts (Open Sans, Exo 2, Montserrat) -->
	<link rel=""stylesheet"" href=""https://fonts.googleapis.com/css?family=Open+Sans:300,300italic,regular,italic,600,600italic,700,700italic,800,800italic|Exo+2:100,200,300,regular,500,600,700,800,900,100italic,200italic,300italic,italic,500italic,600italic,700italic,800italic,900italic|Montserrat:100,100italic,200,200italic,300,300italic,regular,italic,500,500italic,600,600italic,700,700italic,800,800italic,900,900italic&subset=latin,latin-ext"">

	<!-- jQuery -->
	<script src=""/lib/jquery/dist/jquery.min.js""></script>
</head>";

        // Match from any comment/meta before <head> all the way through </head>
        html = Regex.Replace(html,
            @"(?s)(<!--.*?-->\s*)?((<meta[^>]*content-type[^>]*/><!--[^>]*-->)\s*)?<head>.*?</head>",
            newHead);

        // ── 2. Remove cfasync email-decode script ──
        html = Regex.Replace(html,
            @"(?s)\s*<script data-cfasync=""false""\s*\n?\s*src=""[^""]*email-decode[^""]*""></script>",
            "");

        // ── 3. Remove speculationrules block ──
        html = Regex.Replace(html,
            @"(?s)\s*<script type=""speculationrules"">.*?</script>",
            "");

        // ── 4. Remove wpb-modifications text/html script ──
        html = Regex.Replace(html,
            @"\s*<script type=""text/html""[^>]*>.*?</script>",
            "");

        // ── 5. Remove leftover Roboto font link ──
        html = Regex.Replace(html,
            @"\s*<link[^>]*fonts\.googleapis\.com[^>]*Roboto[^>]*/?>",
            "");

        // ── 6. Remove empty <script></script> ──
        html = Regex.Replace(html,
            @"\s*<script></script>",
            "");

        // ── 7. Remove Instagram Feed JS comment (its script was already stripped) ──
        html = html.Replace("\n\t<!-- Instagram Feed JS -->\n\t", "\n\t");

        // ── 8. Remove HTTrack mirror comments and meta at very top ──
        html = Regex.Replace(html,
            @"(?s)\s*<!-- Mirrored from [^>]*-->\s*<!-- Added by HTTrack -->\s*<meta[^>]*content-type[^>]*/?><!-- /Added by HTTrack -->",
            "");

        // ── 9. Remove trailing WP cache comments at bottom ──
        html = Regex.Replace(html,
            @"(?s)\s*<!-- Mirrored from[^>]*-->\s*\n*<!-- Dynamic page[^-]*-->\s*\n*<!-- Cached page[^-]*-->\s*\n*<!-- super cache -->",
            "");

        File.WriteAllText(path, html.TrimEnd() + "\n", Encoding.UTF8);
        int lines = html.Split('\n').Length;
        Console.WriteLine("Done. File now ~" + lines + " lines.");
    }
}
'@

[LandingCleaner]::Run('c:\Users\HP OMEN\sources\repos\Return Trip\Return trip\Views\Home\Landing.cshtml')
