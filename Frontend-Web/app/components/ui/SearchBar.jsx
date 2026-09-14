import React, { useState, useEffect } from "react";
import styles from "./Search.module.css";
import SearchIcon from "@mui/icons-material/Search";
import { WhiteLoading } from "./Loading";

const ENDPOINTS = {
  all: "/Search",
  user: "/Search/user",
  destination: "/Search/destination",
  trip: "/Search/trip",
  event: "/Search/event",
};

const TYPE_COLORS = {
  destinations: "var(--Mid-Background)",
  trips: "var(--Blue-Perfect)",
  users: "var(--Neutrals-Medium-Outline)",
  events: "var(--Burgundy-Perfect)",
};

const TYPE_LABELS = {
  destinations: "Destination",
  trips: "Trip",
  users: "User",
  events: "Event",
};

const SearchBar = ({ searchType = "all", size = "default" }) => {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
  const [query, setQuery] = useState("");
  const [results, setResults] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [activeFilter, setActiveFilter] = useState("all"); // New state for active filter

  const endpoint = ENDPOINTS[searchType] || ENDPOINTS.all;

  useEffect(() => {
    if (!query) {
      setResults(null);
      return;
    }

    const delayDebounceFn = setTimeout(() => {
      fetchResults();
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [query]);

  const fetchResults = async () => {
    try {
      setIsLoading(true);
      const url =
        searchType === "all"
          ? `${baseUrl}${endpoint}?query=${query}&mode=wildcard`
          : `${baseUrl}${endpoint}?query=${query}`;

      const res = await fetch(url);
      const data = await res.json();

      if (searchType === "destination") {
        setResults(data.destinations || []);
      } else if (searchType === "user") {
        setResults(data.users || []);
      } else if (searchType === "trip") {
        setResults(data.trips || []);
      } else if (searchType === "event") {
        setResults(data.events || []);
      } else {
        setResults(data); // For "all"
        setActiveFilter("all"); // Reset filter when new search
      }
    } catch (error) {
      console.error("Search error:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleChange = (e) => {
    setQuery(e.target.value);
  };

  const handleFilterClick = (filterType) => {
    setActiveFilter(filterType);
  };

  // Calculate counts for each section
  const getCounts = () => {
    if (!results) return null;

    return {
      all:
        (results.users?.length || 0) +
        (results.destinations?.length || 0) +
        (results.trips?.length || 0) +
        (results.events?.length || 0),
      users: results.users?.length || 0,
      destinations: results.destinations?.length || 0,
      trips: results.trips?.length || 0,
      events: results.events?.length || 0,
    };
  };

  const counts = getCounts();

  return (
    <div className={styles.searchContainer}>
      <div className={styles.inputWrapper}>
        <input
          type="text"
          placeholder={`Search ${
            searchType !== "all" ? searchType : "anything"
          }...`}
          value={query}
          onChange={handleChange}
          className={`${styles.searchInput} ${
            size === "large" ? styles.searchInputLarge : ""
          }`}
        />
        <div className={styles.searchIcon}>
          {isLoading ? <WhiteLoading /> : <SearchIcon sx={{ fontSize: 34 }} />}
        </div>
      </div>
      {results && searchType === "all" && (
        <div className={styles.searchResults}>
          {/* Results Counter and Filter */}
          <div className={styles.resultsHeader}>
            <div className={styles.filterTabs}>
              <button
                className={`${styles.filterTab} ${
                  activeFilter === "all" ? styles.active : ""
                }`}
                onClick={() => handleFilterClick("all")}
              >
                All Results <span>{counts.all}</span>
              </button>
              <button
                className={`${styles.filterTab} ${
                  activeFilter === "destinations" ? styles.active : ""
                }`}
                onClick={() => handleFilterClick("destinations")}
              >
                Destinations <span>{counts.destinations}</span>
              </button>
              <button
                className={`${styles.filterTab} ${
                  activeFilter === "trips" ? styles.active : ""
                }`}
                onClick={() => handleFilterClick("trips")}
              >
                Trips <span>{counts.trips}</span>
              </button>
              <button
                className={`${styles.filterTab} ${
                  activeFilter === "users" ? styles.active : ""
                }`}
                onClick={() => handleFilterClick("users")}
              >
                Users <span>{counts.users}</span>
              </button>
              <button
                className={`${styles.filterTab} ${
                  activeFilter === "events" ? styles.active : ""
                }`}
                onClick={() => handleFilterClick("events")}
              >
                Events <span>{counts.events}</span>
              </button>
            </div>
          </div>

          {/* Filtered Results - this part remains exactly the same */}
          {activeFilter === "all" && (
            <>
              {counts.destinations > 0 && (
                <Section data={results.destinations} type="destinations" />
              )}
              {counts.trips > 0 && (
                <Section data={results.trips} type="trips" />
              )}
              {counts.users > 0 && (
                <Section data={results.users} type="users" />
              )}
              {counts.events > 0 && (
                <Section data={results.events} type="events" />
              )}
            </>
          )}

          {activeFilter === "destinations" && counts.destinations > 0 && (
            <Section data={results.destinations} type="destinations" />
          )}
          {activeFilter === "trips" && counts.trips > 0 && (
            <Section data={results.trips} type="trips" />
          )}
          {activeFilter === "users" && counts.users > 0 && (
            <Section data={results.users} type="users" />
          )}
          {activeFilter === "events" && counts.events > 0 && (
            <Section data={results.events} type="events" />
          )}
        </div>
      )}
      {results && searchType !== "all" && (
        <div className={styles.searchResults}>
          <Section
            title={searchType}
            data={results}
            renderItem={(item) => item.name || item.title}
          />
        </div>
      )}
    </div>
  );
};

const Section = ({ data, type }) => {
  if (!data || data.length === 0) return null;

  const handleItemClick = (item) => {
    if (type === "destinations") {
      window.location.href = `/explore/${item.id}`;
    }
    // Add similar handlers for other types if needed
  };

  return (
    <div className={styles.section}>
      <ul>
        {data.map((item) => (
          <li key={item.id} onClick={() => handleItemClick(item)}>
            <span className={styles.resultContent}>
              {item.name || item.title || `${item.firstName} ${item.lastName}`}
            </span>
            <span
              className={styles.typeTag}
              style={{ backgroundColor: TYPE_COLORS[type] }}
            >
              {TYPE_LABELS[type]}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SearchBar;
