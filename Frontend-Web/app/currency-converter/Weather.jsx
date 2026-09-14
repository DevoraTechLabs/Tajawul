"use client";
import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import styles from "./WeatherPopup.module.css";
import API from "@/utils/api";
import {
  WbSunny,
  Cloud,
  Opacity,
  AcUnit,
  Thunderstorm,
  WbTwilight,
  BeachAccess,
  Air,
  Compress,
  WaterDrop,
  LocationOff,
  ArrowForward
} from "@mui/icons-material";

const WeatherPopup = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [city, setCity] = useState("");
  const [weather, setWeather] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Weather icon animation variants
  const iconVariants = {
    hidden: { scale: 0, opacity: 0 },
    visible: { 
      scale: 1, 
      opacity: 1,
      transition: { type: "spring", stiffness: 300, damping: 15 }
    },
    hover: { scale: 1.1 }
  };

  const getWeatherIcon = (condition) => {
    if (!condition) return <WbSunny className={styles.sunny} />;
    
    const lowerCondition = condition.toLowerCase();
    
    if (lowerCondition.includes('clear')) return (
      <motion.div variants={iconVariants}>
        <WbSunny className={styles.sunny} />
      </motion.div>
    );
    if (lowerCondition.includes('cloud')) return (
      <motion.div variants={iconVariants}>
        <Cloud className={styles.cloudy} />
      </motion.div>
    );
    if (lowerCondition.includes('rain')) return (
      <motion.div variants={iconVariants}>
        <Opacity className={styles.rainy} />
      </motion.div>
    );
    if (lowerCondition.includes('snow')) return (
      <motion.div.div variants={iconVariants}>
        <AcUnit className={styles.snowy} />
      </motion.div.div>
    );
    if (lowerCondition.includes('thunder')) return (
      <motion.div variants={iconVariants}>
        <Thunderstorm className={styles.thunderstorm} />
      </motion.div>
    );
    if (lowerCondition.includes('haze') || lowerCondition.includes('fog')) return (
      <motion.div variants={iconVariants}>
        <BeachAccess className={styles.foggy} />
      </motion.div>
    );
    if (lowerCondition.includes('wind')) return (
      <motion.div variants={iconVariants}>
        <Air className={styles.windy} />
      </motion.div>
    );
    
    return (
      <motion.div variants={iconVariants}>
        <WbTwilight className={styles.defaultIcon} />
      </motion.div>
    );
  };

  const fetchWeather = async () => {
    if (!city.trim()) {
      setError("Please enter a city name");
      return;
    }

    setLoading(true);
    setError("");
    
    try {
      const response = await API.get(`/Weather?city=${encodeURIComponent(city.trim())}`);
      
      if (!response.data?.currentConditions) {
        throw new Error("Invalid weather data");
      }

      const { currentConditions, resolvedAddress, description } = response.data;
      
      setWeather({
        location: resolvedAddress || city,
        temp: currentConditions.temp,
        feelsLike: currentConditions.feelslike,
        condition: currentConditions.conditions,
        humidity: currentConditions.humidity,
        windSpeed: currentConditions.windspeed,
        pressure: currentConditions.pressure,
        description,
        sunrise: currentConditions.sunrise,
        sunset: currentConditions.sunset
      });

    } catch (err) {
      setError(err.response?.status === 404 
        ? `"${city}" not found. Try another location.` 
        : "Failed to fetch weather. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {/* Floating Action Button */}
      <motion.button
        className={styles.fab}
        onClick={() => setIsOpen(true)}
        whileHover={{ scale: 1.05 }}
        whileTap={{ scale: 0.95 }}
      >
        <WbSunny className={styles.fabIcon} />
        <span>Weather</span>
      </motion.button>

      {/* Popup Modal */}
      <AnimatePresence>
        {isOpen && (
          <motion.div
            className={styles.overlay}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={() => setIsOpen(false)}
          >
            <motion.div
              className={styles.modal}
              initial={{ y: 50, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              exit={{ y: 50, opacity: 0 }}
              onClick={(e) => e.stopPropagation()}
            >
              {/* Header */}
              <div className={styles.header}>
                <h2>Weather Forecast</h2>
                <button 
                  className={styles.closeBtn}
                  onClick={() => setIsOpen(false)}
                >
                  &times;
                </button>
              </div>

              {/* Search Form */}
              <form 
                className={styles.searchForm}
                onSubmit={(e) => {
                  e.preventDefault();
                  fetchWeather();
                }}
              >
                <div className={styles.searchGroup}>
                  <input
                    type="text"
                    value={city}
                    onChange={(e) => setCity(e.target.value)}
                    placeholder="Enter city name..."
                    className={styles.searchInput}
                  />
                  <button 
                    type="submit" 
                    className={styles.searchButton}
                    disabled={loading}
                  >
                    {loading ? (
                      <div className={styles.spinner}></div>
                    ) : (
                      <ArrowForward className={styles.searchIcon} />
                    )}
                  </button>
                </div>
                {error && (
                  <motion.div
                    className={styles.error}
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                  >
                    <LocationOff className={styles.errorIcon} />
                    {error}
                  </motion.div>
                )}
              </form>

              {/* Weather Display */}
              {weather && (
                <motion.div
                  className={styles.weatherDisplay}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 0.2 }}
                >
                  {/* Current Weather */}
                  <div className={styles.currentWeather}>
                    <div className={styles.location}>
                      <h3>{weather.location}</h3>
                      <p className={styles.description}>{weather.description}</p>
                    </div>
                    
                    <motion.div 
                      className={styles.weatherMain}
                      initial={{ scale: 0.8 }}
                      animate={{ scale: 1 }}
                    >
                      <div className={styles.weatherIcon}>
                        {getWeatherIcon(weather.condition)}
                      </div>
                      <div className={styles.temp}>
                        <span className={styles.currentTemp}>{weather.temp}°</span>
                        <span className={styles.feelsLike}>Feels like {weather.feelsLike}°</span>
                      </div>
                    </motion.div>
                  </div>

                  {/* Weather Details */}
                  <motion.div 
                    className={styles.weatherDetails}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: 0.4 }}
                  >
                    <div className={styles.detailCard}>
                    <WaterDrop className={`${styles.detailIcon} ${styles.humidity}`} style={{ fontSize: '32px' }} />
                    <div>
                        <span>Humidity</span>
                        <strong>{weather.humidity}%</strong>
                    </div>
                    </div>

                    <div className={styles.detailCard}>
                    <Air className={`${styles.detailIcon} ${styles.wind}`} style={{ fontSize: '32px' }} />
                    <div>
                        <span>Wind</span>
                        <strong>{weather.windSpeed} km/h</strong>
                    </div>
                    </div>

                    <div className={styles.detailCard}>
                    <Compress className={`${styles.detailIcon} ${styles.pressure}`} style={{ fontSize: '32px' }} />
                    <div>
                        <span>Pressure</span>
                        <strong>{weather.pressure} hPa</strong>
                    </div>
                    </div>
                  </motion.div>

                  {/* Sunrise/Sunset */}
                  {weather.sunrise && weather.sunset && (
                    <motion.div 
                      className={styles.sunTimes}
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 0.6 }}
                    >
                      <div className={styles.sunTime}>
                        <WbSunny className={styles.sunIcon} />
                        <span>Sunrise: {weather.sunrise}</span>
                      </div>
                      <div className={styles.sunTime}>
                        <WbSunny className={styles.sunsetIcon} />
                        <span>Sunset: {weather.sunset}</span>
                      </div>
                    </motion.div>
                  )}
                </motion.div>
              )}
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  );
};

export default WeatherPopup;