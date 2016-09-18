/// <reference path="../wwwroot/lib/jquery/dist/jquery.d.ts" />
/// <reference path="../wwwroot/lib/knockout/dist/knockout.d.ts" />
namespace DiscoverHollywood {

    var settings = {
        pageSize: 20
    };

    export interface MovieEntry {
        id: number;
        name: string;
        year: number;
        genres: string[];
    }

    export interface RatingSummaryEntry {
        id: string;
        value: number;
        count: number;
        year: number;
        movie: number;
    }

    export interface MovieGenresEntry {
        id: string;
        movie: number;
        genre: string;
    }

    export interface MovieTagsEntry {
        id: string;
        movie: number;
        value: string;
    }

    function encodeURIParam(value: any): string {
        return value ? encodeURIComponent(value.toString()) : "";
    }

    /**
      * The page model.
      */
    export class PageModel {
        hasFurther = ko.observable(false);
        detail = ko.observable<MovieEntry>();
        rating = ko.observableArray<RatingSummaryEntry>();
        years = ko.observableArray<number>();
        list = ko.observableArray<MovieEntry>();
        page = ko.observable(0);
        year = ko.observable<number>();
        genres = ko.observable<string>();
        tags = ko.observableArray<MovieTagsEntry>();
        loadedTags = ko.observable(false);
        isLoading = ko.observable(false);
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
            this.tags(null);
            history.pushState(null, null, this.getSearchPath());
        }

        getList() {
            return list(this.q(), this.year(), this.genres(), this.page());
        }

        updateList() {
            this.isLoading(true);
            this.page(0);
            var promise = this.getList();
            promise.then((r) => {
                if (!r) return;
                this.list(r);
                this.isLoading(false);
                if (r.length === 1 && !!r[0]) {
                    this.show(r[0].id);
                    return;
                }

                this.hasFurther(r.length >= settings.pageSize);
                history.pushState(null, null, this.getSearchPath());
            }, (r) => {
                this.isLoading(false);
            });
            return promise;
        }

        getSearchPath() {
            var str = "?";
            if (!!this.q()) str += (str.length > 1 ? "&" : "") + "q=" + this.q();
            if (!!this.year()) str += (str.length > 1 ? "&" : "") + "year=" + this.year();
            if (!!this.genres()) str += (str.length > 1 ? "&" : "") + "genres=" + this.genres();
            return str;
        }

        getMore() {
            this.isLoading(true);
            this.page((this.page() || 0) + 1);
            var promise = this.getList();
            promise.then((r) => {
                this.list.push(...r);
                this.isLoading(false);
                this.hasFurther(r.length >= settings.pageSize);
            }, (r) => {
                this.isLoading(false);
            });
        }

        show(id: number) {
            this.rating(null);
            if (id == null) {
                this.detail(null);
                this.tags(null);
                return;
            }

            var selId = getQueryAsInt("id");
            if (selId != id) history.pushState(selId, null, `?id=${id}`);
            var cache = this.list();
            var entry: MovieEntry;
            if (!!cache) cache.some((item) => {
                if (!item || item.id == null || item.id.toString() !== id.toString()) return false;
                entry = item;
                this.detail(entry);
                return true;
            });
            if (!entry) get(id).then((r) => {
                entry = r;
                this.detail(r);
            });
            rating(id).then((r) => {
                this.rating(r);
            });
        }

        showTags() {
            var entry = this.detail();
            if (!entry || entry.id == null) return;
            tags(entry.id).then((r) => {
                var testEntry = this.detail();
                if (!testEntry || testEntry.id !== entry.id) return;
                this.tags(!!r && r.length > 0 ? r : [{ value: "(empty)", movie: entry.id, id: null }]);
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
      * Gets ratings of a specific movie.
      */
    export function rating(id: number): PromiseLike<RatingSummaryEntry[]> {
        return $.getJSON(`/api/movies/${id}/ratings`);
    }

    /**
      * Gets tags of a specific movie.
      */
    export function tags(id: number): PromiseLike<MovieTagsEntry[]> {
        return $.getJSON(`/api/movies/${id}/tags`);
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
            var selId = getQueryAsInt("id");
            model.updateList().then((r) => {
                model.show(selId);
            });
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
