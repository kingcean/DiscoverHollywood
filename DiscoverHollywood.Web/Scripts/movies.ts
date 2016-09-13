/// <reference path="../wwwroot/lib/jquery/dist/jquery.d.ts" />
/// <reference path="../wwwroot/lib/knockout/dist/knockout.d.ts" />
namespace DiscoverHollywood {

    export interface MovieEntry {
        id: string;
        name: string;
        year: number;
        genres: string[];
    }

    function encodeURIParam(value: any): string {
        return value ? encodeURIComponent(value.toString()) : "";
    }

    /**
      * The page model.
      */
    export class PageModel {
        detail = ko.observable<MovieEntry>();
        rating = ko.observable<any>();
        years = ko.observableArray<number>();
        list = ko.observableArray<MovieEntry>();
        page = ko.observable(0);
        year = ko.observable<number>();
        genres = ko.observable<string>();
        q = ko.observable<string>();
        genresList = ["",
            "Action",
            "Adventure",
            "Animation",
            "Children's",
            "Comedy",
            "Crime",
            "Documentary",
            "Drama",
            "Fantasy",
            "Film-Noir",
            "Horror",
            "Musical",
            "Mystery",
            "Romance",
            "Sci-Fi",
            "Thriller",
            "War",
            "Western"
        ];

        constructor() {
            for (var i = 2016; i > 1995; i--) {
                this.years.push(i);
            }
        }

        home() {
            this.detail(null);
            this.rating(null);
        }

        getList() {
            return list(this.q(), this.year(), this.genres(), this.page());
        }

        updateList() {
            this.getList().then((r) => {
                this.list(r);
            });
        }

        getMore() {
            this.page((this.page() || 0) + 1);
            this.getList().then((r) => {
                this.list.push(...r);
            });
        }

        show(id: number) {
            if (id == null) {
                this.detail(null);
                this.rating(null);
                return;
            }

            var selId = getQueryAsInt("id");
            if (selId != id) history.pushState(selId, null, `?id=${id}`);
            get(id).then((r) => {
                this.detail(r);
            });
            rating(id).then((r) => {
                this.rating(r);
            });
        }

        paddingNum(value: number, len: number) {
            var str = value.toString();
            for (var i = str.length; i < len; i++) {
                str = "0" + str;
            }

            return str;
        }
    }

    /**
      * Lists movies with filter.
      */
    export function list(name?: string, year?: number, genres?: string, page?: number): PromiseLike<MovieEntry[]> {
        return $.getJSON(`/api/movies/?q=${encodeURIParam(name)}&year=${encodeURIParam(year)}&genres=${encodeURIParam(genres)}&page=${encodeURIComponent(page != null ? page.toString() : "")}`); 
    }

    /**
      * Gets a specific movie entry.
      */
    export function get(id: number): PromiseLike<MovieEntry> {
        return $.getJSON(`/api/movies/${id}`); 
    }

    /**
      * Gets a specific movie entry.
      */
    export function rating(id: number): PromiseLike<MovieEntry> {
        return $.getJSON(`/api/movies/${id}/ratings`);
    }

    /**
      * Gets query property by given name from URL.
      * @param name  the property name.
      */
    export function getQuery(name: string | number): string {
        if (name == null) return null;
        try {
            if (typeof name === "string") {
                var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
                if (result == null || result.length < 1) {
                    return "";
                }

                return result[1];
            } else if (typeof name === "number") {
                var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
                if (result == null) {
                    return "";
                }

                return result[name].substring(1);
            }
        } catch (ex) { }

        return null;
    }

    export function getQueryAsInt(name: string | number): number {
        var str = getQuery(name);
        try {
            var num = parseInt(str, 10);
            return num != null && !isNaN(num) ? num : undefined;
        } catch (ex) {
            return undefined;
        }
    }

    function homepage() {
        var model = new PageModel();
        var proc = () => {
            model.page(getQueryAsInt("page") || 0);
            model.q(getQuery("q"));
            model.year(getQueryAsInt("year"));
            model.genres(getQuery("genres"));
            model.updateList();
            var selId = getQueryAsInt("id");
            model.show(selId);
        };
        proc();
        var container = document.getElementById("page_container");
        ko.applyBindings(model, container);
        container.style.display = "";
        window.addEventListener("popstate", (ev) => {
            proc();
        });
    }

    homepage();
}
