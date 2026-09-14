"use client";
import { useEffect, useRef, useState } from "react";
import mapboxgl from "mapbox-gl";
import styles from "@/forms.module.css";
import AddLocationAltIcon from "@mui/icons-material/AddLocationAlt";
import HighlightOffIcon from "@mui/icons-material/HighlightOff";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import RemoveCircleOutlineIcon from "@mui/icons-material/RemoveCircleOutline";
import SearchIcon from "@mui/icons-material/Search";
import SubdirectoryArrowLeftIcon from "@mui/icons-material/SubdirectoryArrowLeft";

mapboxgl.accessToken = process.env.NEXT_PUBLIC_MAPBOX_TOKEN;

export default function LocationPicker({
  onLocationSelect,
  initialLocation = { lng: 0, lat: 0, address: "" },
  language = "en",
}) {
  // Refs
  const mapContainer = useRef(null);
  const mapRef = useRef(null);
  const markerRef = useRef(null);
  const modalRef = useRef(null);
  const searchInputRef = useRef(null);
  const popupRef = useRef(null);

  // State
  const [showModal, setShowModal] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState("");
  const [showSearchInput, setShowSearchInput] = useState(false);

  // Close modal when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (modalRef.current && !modalRef.current.contains(event.target)) {
        setShowModal(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Initialize Map
  useEffect(() => {
    if (!showModal || !mapContainer.current) return;

    const map = new mapboxgl.Map({
      container: mapContainer.current,
      style: "mapbox://styles/mapbox/streets-v12",
      center: [initialLocation.lng || 0, initialLocation.lat || 0],
      zoom: initialLocation.lng ? 12 : 1.5,
      minZoom: 1,
      maxZoom: 18,
      projection: "globe",
      glyphs: "mapbox://fonts/mapbox/{fontstack}/{range}.pbf?language=ar",
      localIdeographFontFamily: "'Noto Sans Arabic', sans-serif",
    });

    mapRef.current = map;

    // Marker Setup
    const markerEl = document.createElement("div");
    markerEl.className = styles.customMarker;
    const marker = new mapboxgl.Marker({
      element: markerEl,
      anchor: "bottom",
    });

    if (initialLocation.lng && initialLocation.lat) {
      marker.setLngLat([initialLocation.lng, initialLocation.lat]).addTo(map);
    }
    markerRef.current = marker;

    // Map Events
    map.on("style.load", () => {
      map.setFog({});
      map.setLayoutProperty("poi-label", "visibility", "visible");
      map.setLayoutProperty("road-label", "visibility", "visible");

      if (initialLocation.lng) {
        map.flyTo({
          center: [initialLocation.lng, initialLocation.lat],
          zoom: 14,
          pitch: 45,
        });
      }
    });

    map.on("click", async (e) => {
      try {
        setIsLoading(true);
        const { lng, lat } = e.lngLat;

        const response = await fetch(
          `https://api.mapbox.com/geocoding/v5/mapbox.places/${lng},${lat}.json?language=${language}&access_token=${mapboxgl.accessToken}`
        );
        const data = await response.json();
        const address = data.features[0]?.place_name || "Selected Location";

        updateLocation(lng, lat, address);

        // Close the modal after selection
        setTimeout(() => setShowModal(false), 500);
      } catch (error) {
        console.error("Error:", error);
      } finally {
        setIsLoading(false);
      }
    });

    map.on("load", () => setIsLoading(false));

    return () => {
      if (mapRef.current) mapRef.current.remove();
      if (popupRef.current) popupRef.current.remove();
    };
  }, [showModal, initialLocation.lng, initialLocation.lat, language]);

  // Helper Functions
  const updateLocation = (lng, lat, address) => {
    if (markerRef.current) {
      markerRef.current.remove();
    }

    const markerEl = document.createElement("div");
    markerEl.className = styles.customMarker;
    const marker = new mapboxgl.Marker({
      element: markerEl,
      anchor: "bottom",
    })
      .setLngLat([lng, lat])
      .addTo(mapRef.current);

    markerRef.current = marker;

    mapRef.current.flyTo({
      center: [lng, lat],
      zoom: 14,
      essential: true,
    });

    const popupNode = document.createElement("div");
    popupNode.style.fontFamily = "'Noto Sans Arabic', sans-serif";
    popupNode.style.fontFeatureSettings = "'calt' 1, 'liga' 1";
    popupNode.style.direction = language === "ar" ? "rtl" : "ltr";
    popupNode.style.textAlign = language === "ar" ? "right" : "left";
    popupNode.style.fontSize = "14px";
    popupNode.style.lineHeight = "1.5";
    popupNode.textContent = address;

    // Remove previous popup if exists
    if (popupRef.current) popupRef.current.remove();

    popupRef.current = new mapboxgl.Popup({ closeButton: false })
      .setLngLat([lng, lat])
      .setDOMContent(popupNode)
      .addTo(mapRef.current);

    onLocationSelect({ lng, lat, address });
  };

  // Search Functions
  const handleSearch = async (e) => {
    e?.preventDefault();
    if (!searchQuery.trim() || !mapRef.current) return;

    try {
      setIsLoading(true);
      const response = await fetch(
        `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(
          searchQuery
        )}.json?language=${language}&access_token=${mapboxgl.accessToken}`
      );
      const data = await response.json();

      if (data.features?.length > 0) {
        const [lng, lat] = data.features[0].center;
        const address = data.features[0].place_name;
        updateLocation(lng, lat, address);
      }
    } catch (error) {
      console.error("Search error:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSearch(e);
    }
  };

  // Zoom Controls
  const zoomIn = () => mapRef.current?.zoomIn();
  const zoomOut = () => mapRef.current?.zoomOut();

  return (
    <div className={styles.mapIconContainer}>
      <button
        onClick={() => setShowModal(true)}
        className={styles.mapIconButton}
        aria-label="Select location on map"
      >
        <AddLocationAltIcon
          sx={{
            fontSize: "37px",
            color: "var(--Green-Perfect)",
            transition: "all 0.2s ease",
            "&:hover": {
              transform: "scale(1.1)",
              color: "var(--Green-Hover)",
            },
          }}
        />
      </button>

      {showModal && (
        <div className={styles.mapModal}>
          <div className={styles.mapModalContent} ref={modalRef}>
            <button
              onClick={() => setShowModal(false)}
              className={styles.closeButton}
              aria-label="Close map"
            >
              <HighlightOffIcon sx={{ fontSize: 24, color: "#666" }} />
            </button>

            <div className={styles.mapSearchContainer}>
              <div
                className={`${styles.mapSearchWrapper} ${
                  showSearchInput ? styles.expanded : ""
                }`}
                onMouseEnter={() => setShowSearchInput(true)}
                onMouseLeave={() =>
                  !searchInputRef.current?.contains(document.activeElement) &&
                  setShowSearchInput(false)
                }
                dir={language === "ar" ? "rtl" : "ltr"}
              >
                <SearchIcon className={styles.mapSearchIcon} />
                {showSearchInput && (
                  <div className={styles.mapSearchInputWrapper}>
                    <input
                      ref={searchInputRef}
                      type="text"
                      value={searchQuery}
                      onChange={(e) => setSearchQuery(e.target.value)}
                      onKeyDown={handleKeyDown}
                      placeholder={
                        language === "ar"
                          ? "ابحث عن موقع..."
                          : "Search for a place..."
                      }
                      className={styles.mapSearchInput}
                      dir={language === "ar" ? "rtl" : "ltr"}
                      autoFocus
                    />
                    <button
                      onClick={handleSearch}
                      className={styles.mapSearchButton}
                      aria-label="Search location"
                    >
                      <SubdirectoryArrowLeftIcon
                        className={styles.mapSearchArrowIcon}
                      />
                    </button>
                  </div>
                )}
              </div>
            </div>

            <div className={styles.mapWrapper}>
              {isLoading && (
                <div className={styles.mapLoading}>Loading map...</div>
              )}
              <div ref={mapContainer} className={styles.mapContainer} />

              {/* Zoom Controls - Now Visible */}
              <div className={styles.mapZoomControls}>
                <button
                  onClick={zoomIn}
                  className={styles.mapZoomButton}
                  aria-label="Zoom in"
                >
                  <AddCircleOutlineIcon
                    sx={{
                      fontSize: "20px",
                      color: "var(--Neutrals-Black-Text)",
                    }}
                  />
                </button>
                <button
                  onClick={zoomOut}
                  className={styles.mapZoomButton}
                  aria-label="Zoom out"
                >
                  <RemoveCircleOutlineIcon
                    sx={{
                      fontSize: "20px",
                      color: "var(--Neutrals-Black-Text)",
                    }}
                  />
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
