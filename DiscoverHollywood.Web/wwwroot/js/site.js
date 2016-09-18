/// <reference path="../wwwroot/lib/jquery/dist/jquery.d.ts" />
/// <reference path="../wwwroot/lib/knockout/dist/knockout.d.ts" />
var DiscoverHollywood;
(function (DiscoverHollywood) {
    var settings = {
        pageSize: 20
    };
    function encodeURIParam(value) {
        return value ? encodeURIComponent(value.toString()) : "";
    }
    /**
      * The page model.
      */
    var PageModel = (function () {
        function PageModel() {
            this.hasFurther = ko.observable(false);
            this.detail = ko.observable();
            this.rating = ko.observableArray();
            this.years = ko.observableArray();
            this.list = ko.observableArray();
            this.page = ko.observable(0);
            this.year = ko.observable();
            this.genres = ko.observable();
            this.tags = ko.observableArray();
            this.loadedTags = ko.observable(false);
            this.isLoading = ko.observable(false);
            this.q = ko.observable();
            this.genresList = ["",
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
            for (var i = 2016; i > 1995; i--) {
                this.years.push(i);
            }
        }
        PageModel.prototype.home = function () {
            this.detail(null);
            this.rating(null);
            this.tags(null);
            history.pushState(null, null, this.getSearchPath());
        };
        PageModel.prototype.getList = function () {
            return list(this.q(), this.year(), this.genres(), this.page());
        };
        PageModel.prototype.updateList = function () {
            var _this = this;
            this.isLoading(true);
            this.page(0);
            var promise = this.getList();
            promise.then(function (r) {
                if (!r)
                    return;
                _this.list(r);
                _this.isLoading(false);
                if (r.length === 1 && !!r[0]) {
                    _this.show(r[0].id);
                    return;
                }
                _this.hasFurther(r.length >= settings.pageSize);
                history.pushState(null, null, _this.getSearchPath());
            }, function (r) {
                _this.isLoading(false);
            });
            return promise;
        };
        PageModel.prototype.getSearchPath = function () {
            var str = "?";
            if (!!this.q())
                str += (str.length > 1 ? "&" : "") + "q=" + this.q();
            if (!!this.year())
                str += (str.length > 1 ? "&" : "") + "year=" + this.year();
            if (!!this.genres())
                str += (str.length > 1 ? "&" : "") + "genres=" + this.genres();
            return str;
        };
        PageModel.prototype.getMore = function () {
            var _this = this;
            this.isLoading(true);
            this.page((this.page() || 0) + 1);
            var promise = this.getList();
            promise.then(function (r) {
                (_a = _this.list).push.apply(_a, r);
                _this.isLoading(false);
                _this.hasFurther(r.length >= settings.pageSize);
                var _a;
            }, function (r) {
                _this.isLoading(false);
            });
        };
        PageModel.prototype.show = function (id) {
            var _this = this;
            this.rating(null);
            if (id == null) {
                this.detail(null);
                this.tags(null);
                return;
            }
            var selId = getQueryAsInt("id");
            if (selId != id)
                history.pushState(selId, null, "?id=" + id);
            var cache = this.list();
            var entry;
            if (!!cache)
                cache.some(function (item) {
                    if (!item || item.id == null || item.id.toString() !== id.toString())
                        return false;
                    entry = item;
                    _this.detail(entry);
                    return true;
                });
            if (!entry)
                get(id).then(function (r) {
                    entry = r;
                    _this.detail(r);
                });
            rating(id).then(function (r) {
                _this.rating(r);
            });
        };
        PageModel.prototype.showTags = function () {
            var _this = this;
            var entry = this.detail();
            if (!entry || entry.id == null)
                return;
            tags(entry.id).then(function (r) {
                var testEntry = _this.detail();
                if (!testEntry || testEntry.id !== entry.id)
                    return;
                _this.tags(!!r && r.length > 0 ? r : [{ value: "(empty)", movie: entry.id, id: null }]);
            });
        };
        PageModel.prototype.paddingNum = function (value, len) {
            var str = value.toString();
            for (var i = str.length; i < len; i++) {
                str = "0" + str;
            }
            return str;
        };
        return PageModel;
    }());
    DiscoverHollywood.PageModel = PageModel;
    /**
      * Lists movies with filter.
      */
    function list(name, year, genres, page) {
        return $.getJSON("/api/movies/?q=" + encodeURIParam(name) + "&year=" + encodeURIParam(year) + "&genres=" + encodeURIParam(genres) + "&page=" + encodeURIComponent(page != null ? page.toString() : ""));
    }
    DiscoverHollywood.list = list;
    /**
      * Gets a specific movie entry.
      */
    function get(id) {
        return $.getJSON("/api/movies/" + id);
    }
    DiscoverHollywood.get = get;
    /**
      * Gets ratings of a specific movie.
      */
    function rating(id) {
        return $.getJSON("/api/movies/" + id + "/ratings");
    }
    DiscoverHollywood.rating = rating;
    /**
      * Gets tags of a specific movie.
      */
    function tags(id) {
        return $.getJSON("/api/movies/" + id + "/tags");
    }
    DiscoverHollywood.tags = tags;
    /**
      * Gets query property by given name from URL.
      * @param name  the property name.
      */
    function getQuery(name) {
        if (name == null)
            return null;
        try {
            if (typeof name === "string") {
                var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
                if (result == null || result.length < 1) {
                    return "";
                }
                return result[1];
            }
            else if (typeof name === "number") {
                var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
                if (result == null) {
                    return "";
                }
                return result[name].substring(1);
            }
        }
        catch (ex) { }
        return null;
    }
    DiscoverHollywood.getQuery = getQuery;
    function getQueryAsInt(name) {
        var str = getQuery(name);
        try {
            var num = parseInt(str, 10);
            return num != null && !isNaN(num) ? num : undefined;
        }
        catch (ex) {
            return undefined;
        }
    }
    DiscoverHollywood.getQueryAsInt = getQueryAsInt;
    function homepage() {
        var model = new PageModel();
        var proc = function () {
            model.page(getQueryAsInt("page") || 0);
            model.q(getQuery("q"));
            model.year(getQueryAsInt("year"));
            model.genres(getQuery("genres"));
            var selId = getQueryAsInt("id");
            model.updateList().then(function (r) {
                model.show(selId);
            });
        };
        proc();
        var container = document.getElementById("page_container");
        ko.applyBindings(model, container);
        container.style.display = "";
        window.addEventListener("popstate", function (ev) {
            proc();
        });
    }
    homepage();
})(DiscoverHollywood || (DiscoverHollywood = {}));
