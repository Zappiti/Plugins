# Plugins

Hi,

This website is dedicated to Zappiti Media Center v4 plugins.

It provide a couple of projects samples that allow to create a custom movie/tvshow scraper.

Basically, your plugin must provide a class that implement IMovieScraper / ITvShowScraper interface.

It must be a .NET 4.5 C# library.

I advise you to fork this project and to create a new project for your plugin.
Then, you can create a pull request when your work is done to get a review.
If everything is OK, I'll add the plugin in the next version of Zappiti.

For now, there is no easy way to test your plugin in Zappiti on your own,
but we can help you to test your library during the pull request.

We'll try to make it easy to test it with a running Zappiti Server in a next version too.

Zappiti Team
