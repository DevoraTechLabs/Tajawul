'use client';
import { useEffect, useRef } from 'react';
import mapboxgl from 'mapbox-gl';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import RemoveCircleOutlineIcon from '@mui/icons-material/RemoveCircleOutline';
import PlaceIcon from '@mui/icons-material/Place';

mapboxgl.accessToken = process.env.NEXT_PUBLIC_MAPBOX_TOKEN;

export default function LocationViewer({ lng, lat, address, language = 'en' }) {
  const mapContainer = useRef(null);
  const mapRef = useRef(null);
  const markerRef = useRef(null);
  const popupRef = useRef(null);

  // Styles object (unchanged from your original)
  const styles = {
    mapViewerContainer: {
      height: '450px',
      width: '100%',
      position: 'relative',
      borderRadius: '8px',
      overflow: 'hidden',
      border: '1px solid var(--Neutrals-Light-Outline)',
    },
    mapContainer: {
      height: '100%',
      width: '100%'
    },
    mapZoomControls: {
      position: 'absolute',
      right: '10px',
      bottom: '10px',
      display: 'flex',
      flexDirection: 'column',
      gap: '8px',
      zIndex: 1
    },
    mapZoomButton: {
      background: 'white',
      border: 'none',
      width: '30px',
      height: '30px',
      borderRadius: '4px',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      cursor: 'pointer',
      boxShadow: '0 1px 4px rgba(0,0,0,0.2)'
    },
    customMarker: {
      width: '24px',
      height: '24px',
      backgroundColor: 'var(--Green-Perfect)',
      borderRadius: '50%',
      border: '2px solid white',
      cursor: 'pointer'
    },
    mapPopup: {
      fontFamily: "'Noto Sans Arabic', sans-serif",
      padding: '8px',
      fontSize: '14px',
      direction: language === 'ar' ? 'rtl' : 'ltr',
      textAlign: language === 'ar' ? 'right' : 'left'
    },
    mapIconButton: {
      position: 'absolute',
      top: '12px',
      right: '12px',
      background: 'white',
      borderRadius: '50%',
      width: '32px',
      height: '32px',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      border: 'none',
      cursor: 'pointer',
      zIndex: 2,
      boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
    }
  };

  useEffect(() => {
    if (!mapContainer.current) return;

    const map = new mapboxgl.Map({
      container: mapContainer.current,
      style: 'mapbox://styles/mapbox/streets-v12',
      center: [lng, lat],
      zoom: 14,
      minZoom: 1,
      maxZoom: 18,
      projection: 'globe',
      glyphs: 'mapbox://fonts/mapbox/{fontstack}/{range}.pbf?language=ar',
      localIdeographFontFamily: "'Noto Sans Arabic', sans-serif",
      locale: {
        'place-city': { 'text-size': 16, 'text-max-width': 6 },
        'place-town': { 'text-size': 14, 'text-max-width': 6 },
        'place-village': { 'text-size': 12, 'text-max-width': 6 },
        'place-neighborhood': { 'text-size': 11, 'text-max-width': 8 }
      }
    });

    mapRef.current = map;

    // Create marker element with inline styles
    const markerEl = document.createElement('div');
    Object.assign(markerEl.style, styles.customMarker);
    const marker = new mapboxgl.Marker({
      element: markerEl,
      anchor: 'bottom'
    }).setLngLat([lng, lat]).addTo(map);
    markerRef.current = marker;

    // Enhanced label visibility control
    map.on('style.load', () => {
      map.setFog({});
      
      setTimeout(() => {
        // List of all possible label layer types
        const labelLayers = [
          'country', 'state', 'county', 'settlement', 'locality',
          'neighborhood', 'landform', 'landuse', 'water', 'waterway',
          'poi', 'airport', 'transit', 'road', 'street'
        ];

        // Process each layer type
        labelLayers.forEach(type => {
          const layers = map.getStyle().layers.filter(layer => 
            layer.id.includes(`${type}-label`) || 
            layer.id.includes(`place-${type}`)
          );
          
          layers.forEach(layer => {
            try {
              // Ensure layer is visible
              map.setLayoutProperty(layer.id, 'visibility', 'visible');
              
              // Enhance text properties
              if (layer.layout && layer.layout['text-field']) {
                map.setLayoutProperty(layer.id, 'text-size', 14);
                map.setLayoutProperty(layer.id, 'text-max-width', 6);
                
                // Prioritize Arabic names
                map.setLayoutProperty(layer.id, 'text-field', [
                  'coalesce',
                  ['get', `name_${language}`],
                  ['get', 'name']
                ]);
              }
            } catch (e) {
              console.warn(`Couldn't modify layer ${layer.id}:`, e);
            }
          });
        });

        // Special handling for Arabic text
        if (language === 'ar') {
          try {
            map.setLayoutProperty('country-label', 'text-font', ['Noto Sans Arabic Regular']);
            map.setLayoutProperty('place-label', 'text-font', ['Noto Sans Arabic Regular']);
          } catch (e) {
            console.warn('Arabic font adjustment failed:', e);
          }
        }
      }, 800); // Increased delay for more reliable layer loading
    });

    // Create popup with inline styles
    const popupNode = document.createElement('div');
    Object.assign(popupNode.style, styles.mapPopup);
    popupNode.textContent = address;

    popupRef.current = new mapboxgl.Popup({ offset: 25, closeButton: false })
      .setLngLat([lng, lat])
      .setDOMContent(popupNode)
      .addTo(map);

    return () => {
      if (mapRef.current) mapRef.current.remove();
      if (popupRef.current) popupRef.current.remove();
    };
  }, [lng, lat, address, language]);

  const zoomIn = () => mapRef.current?.zoomIn();
  const zoomOut = () => mapRef.current?.zoomOut();

  return (
    <div style={styles.mapViewerContainer}>
      <div ref={mapContainer} style={styles.mapContainer} />
      
      <div style={styles.mapZoomControls}>
        <button onClick={zoomIn} style={styles.mapZoomButton}>
          <AddCircleOutlineIcon sx={{ fontSize: '20px', color: 'var(--Neutrals-Black-Text)' }} />
        </button>
        <button onClick={zoomOut} style={styles.mapZoomButton}>
          <RemoveCircleOutlineIcon sx={{ fontSize: '20px', color: 'var(--Neutrals-Black-Text)' }} />
        </button>
      </div>
    </div>
  );
}