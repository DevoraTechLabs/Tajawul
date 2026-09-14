"use client";

import { useState, useEffect } from 'react';
import React from 'react';
import Image from 'next/image';
import Divider from '@mui/material/Divider';
import Avatar from '@mui/material/Avatar';
import MapIcon from '@mui/icons-material/Map';
import EditIcon from '@mui/icons-material/Edit';
import Stats from '@/components/ui/TripStats';
import styles from '@/trip.module.css';
import TripInteractions from './TripInteractions';
import TripIdHandler from '@/components/TripIdHandler';
import SameCountry from '@/components/ui/SameCountry';
import PriceRange from './tags/PriceRange';
import TripDuration from './tags/TripDuration';
import Status from './tags/Status';
import EditTags from '@/components/ui/TripAddOrEditTags';
import Tag from './tags/Tag';
import API from '@/utils/api';
import useAuthStore from '@/store/authStore';
import typeIconsMapping from '@/utils/typeIconsMapping';
import StarIcon from '@mui/icons-material/Star';
import StarBorderIcon from '@mui/icons-material/StarBorder';
import StarHalfIcon from '@mui/icons-material/StarHalf';
import Link from 'next/link';
import LocationViewer from '@/components/map/LocationViewer';
import useTripInteractionsStore from "@/store/TripInteractionsStore";
import TripStatsHydrator from '@/components/TripStatsHydrator';
import { useRouter } from 'next/navigation';


const RatingStars = ({ rating }) => {
  if (rating === undefined || rating === null) {
    return null;
  }

  const normalizedRating = Math.max(0, Math.min(5, rating));
  const fullStars = Math.floor(normalizedRating);
  const hasHalfStar = normalizedRating % 1 >= 0.5;
  const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

  return (
    <div className={styles.ratingContainer}>
      <div className={styles.ratingStars}>
        {[...Array(fullStars)].map((_, i) => (
          <StarIcon key={`full-${i}`} className={styles.starFilled} />
        ))}
        {hasHalfStar && <StarHalfIcon key="half" className={styles.starHalf} />}
        {[...Array(emptyStars)].map((_, i) => (
          <StarBorderIcon key={`empty-${i}`} className={styles.starEmpty} />
        ))}
      </div>
      {rating > 0 ? (
        <span className={styles.ratingValue}>{normalizedRating.toFixed(1)}</span>
      ) : (
        <span className={styles.noRatingText}>No ratings</span>
      )}
    </div>
  );
};

const getTypeIcon = (type) => {
  const IconComponent = typeIconsMapping[type] || typeIconsMapping.Hotel;
  return <IconComponent className={styles.destinationTypeIcon} />;
};

export default function TripDetails({ tripId }) {

  const [coverImage, setCoverImage] = useState(null);
  const [trip, setTrip] = useState(null);
  const [destinations, setDestinations] = useState([]);
  const [tags, setTags] = useState([]);
  const [contributersData, setContributersData] = useState({ users: [] });
  const [loading, setLoading] = useState(true);
  const { accessToken } = useAuthStore();
  const [selectedLocation, setSelectedLocation] = useState(null);
  const [currentSlide, setCurrentSlide] = useState(0);
  const [slides, setSlides] = useState([]);
  const router = useRouter();


  // Get stats from store
  const { favoritesCount, wishesCount, clonesCount } = useTripInteractionsStore();

  const isValidUrl = (url) => {
    if (!url) return false;
    try {
      new URL(url);
      return true;
    } catch {
      return url.startsWith('/') || url.startsWith('blob:');
    }
  };

  const fetchTripTags = async () => {
    try {
      const tagsRes = await API.get(`/Trip/${tripId}/tags`);
      setTags(tagsRes.data?.data || []);
    } catch (error) {
      console.error("Failed to fetch tags:", error);
    }
  };

  const groupDestinationsByDay = () => {
    const grouped = {};
    destinations.forEach(destination => {
      const day = destination.day || 1;
      if (!grouped[day]) {
        grouped[day] = [];
      }
      grouped[day].push(destination);
    });
    return grouped;
  };

  useEffect(() => {
    if (destinations.length > 0) {
      const destinationImages = destinations
        .filter(dest => dest.images && dest.images.length > 0)
        .map(dest => {
          const randomIndex = Math.floor(Math.random() * dest.images.length);
          return {
            src: dest.images[randomIndex],
            alt: dest.name,
            destinationId: dest.destinationId
          };
        });

      setSlides(destinationImages);

      if (destinationImages.length > 1) {
        const interval = setInterval(() => {
          setCurrentSlide(prev => (prev + 1) % destinationImages.length);
        }, 5000);
        
        return () => clearInterval(interval);
      }
    }
  }, [destinations]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        const [tripRes, destinationsRes, contributersRes] = await Promise.all([
          API.get(`/Trip?TripId=${tripId}`).catch(() => ({ data: { trips: [] }})),
          API.get(`/Trip/${tripId}/destinations`).catch(() => ({ data: [] })),
          API.get(`/Trip/${tripId}/users?Relation=contribute`).catch(() => ({ data: { users: [] }}))
        ]);

        const foundTrip = tripRes.data.trips?.find(t => t.tripId === tripId);
        if (!foundTrip) throw new Error("Trip not found");
        
        setTrip(foundTrip);
        
        if (foundTrip.coverImage && isValidUrl(foundTrip.coverImage)) {
          setCoverImage(foundTrip.coverImage);
        } else {
          setCoverImage(null);
        }

        setDestinations(destinationsRes.data || []);
        setContributersData(contributersRes.data || { users: [] });
        await fetchTripTags();

      } catch (error) {
        console.error("Failed to fetch trip details:", error);
      } finally {
        setLoading(false);
      }
    };

    if (tripId) {
      fetchData();
    }
  }, [tripId, accessToken]);

  const handleTagsUpdate = async () => {
    await fetchTripTags();
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!trip) {
    return <div>Trip not found</div>;
  }

  const destinationsByDay = groupDestinationsByDay();
  const sortedDays = Object.keys(destinationsByDay).sort((a, b) => a - b);

  return (
    <div>
      <TripIdHandler tripId={tripId} />
      
      {/* Hydrate the store with server-side data */}
      <TripStatsHydrator 
        favoritesCount={trip?.favoritesCount || 0} 
        wishesCount={trip?.wishesCount || 0}
        clonesCount={trip?.clonesCount || 0}
      />
      
      <div className={styles.coverWrapper}>
        <div className={styles.tripCover}>
          {coverImage && isValidUrl(coverImage) ? (
            <Image
              src={coverImage}
              alt={`Trip: ${trip.title}`}
              fill
              priority
              unoptimized
              className={styles.tripCoverImage}
            />
          ) : (
            <div className={styles.coverPlaceholder} />
          )}
        </div>
        <div className={styles.coverContent}>
          <div className={styles.buttomContainer}>
            <div className={styles.buttomLeftContainer}>
              <h1 className={styles.tripTitle}>{trip.title}</h1>
            </div>
            <div className={styles.buttomRightContainer}>
              <TripInteractions 
                tripId={tripId} 
                onCoverUpdate={(newCover) => {
                  if (newCover && isValidUrl(newCover)) {
                    setCoverImage(`${newCover}?${Date.now()}`);
                  }
                }} 
              />
            </div>
          </div>
        </div>
      </div>

      <div className={styles.mainContent}>
        <div className={styles.leftColumn}>
          <div id="tags" className={`${styles.section} ${styles.tagsContainer}`}>
            <div className={styles.headerRow}>
              <div className={styles.tagsWrapper}>
                {trip?.priceRange && <PriceRange priceRange={trip.priceRange} />}
                {trip?.tripDuration && <TripDuration durations={[trip.tripDuration]} />}
                {trip?.status && <Status status={trip.status} />}
              </div>
              <div className={styles.divider}></div>
              {tags.length > 0 ? (
                <div className={styles.tagsWrapper}>
                  <Tag options={tags} />
                </div>
              ) : (
                <div className={styles.noTagsMessage}>No tags added yet</div>
              )}
              <div className={styles.editButtonContainer}>
                <EditTags tripId={tripId} onUpdate={handleTagsUpdate} />
              </div>
            </div>
          </div>

          {slides.length > 0 && (
            <div className={`${styles.section} ${styles.sliderSection}`}>
              <h2>Trip Highlights</h2>
              <div className={styles.sliderContainer}>
                <div 
                  className={styles.sliderTrack}
                  style={{ transform: `translateX(-${currentSlide * 100}%)` }}
                >
                  {slides.map((slide, index) => (
                    <div key={`${slide.destinationId}-${index}`} className={styles.slide}>
                      <div className={styles.imageWrapper}>
                        <Image
                          src={slide.src}
                          alt={slide.alt}
                          fill
                          className={styles.slideImage}
                          unoptimized
                          style={{
                            objectFit: 'cover',
                            objectPosition: 'center'
                          }}
                          onError={(e) => {
                            e.currentTarget.src = '/fallback.jpg';
                            e.currentTarget.onerror = null;
                          }}
                        />
                      </div>
                    </div>
                  ))}
                </div>
                
                {slides.length > 1 && (
                  <div className={styles.sliderDots}>
                    {slides.map((_, index) => (
                      <button
                        key={index}
                        className={`${styles.dot} ${index === currentSlide ? styles.activeDot : ''}`}
                        onClick={() => setCurrentSlide(index)}
                      />
                    ))}
                  </div>
                )}
              </div>
            </div>
          )}
          
          <div id="destinations" className={styles.section}>
            <h2>Destinations</h2>
            <br />
            {destinations.length > 0 ? (
              <div className={styles.destinationsContainer}>
                <div className={styles.timelineLine}></div>
                
                {sortedDays.map((day, dayIndex) => (
                  <React.Fragment key={`day-${day}`}>
                    <div className={styles.dayNumber}>Day {day}</div>
                    {destinationsByDay[day].map((destination, destIndex) => (
                      <div key={`${day}-${destIndex}`} className={styles.destinationWrapper}>
                        <div className={styles.destinationTimelineIcon}>
                          {getTypeIcon(destination.type)}
                        </div>
                        <div className={styles.destinationCard}>
                          {destination.locations?.length > 0 ? (
                            <button 
                              className={styles.cardMapIconButton}
                              onClick={(e) => {
                                e.stopPropagation();
                                setSelectedLocation({
                                  lng: destination.locations[0].longitude,
                                  lat: destination.locations[0].latitude,
                                  address: `${destination.name}, ${destination.city}`
                                });
                              }}
                              aria-label="View on map"
                            >
                              <MapIcon sx={{ 
                                fontSize: '16px', 
                                color: 'var(--Green-Perfect)',
                                transition: 'transform 0.2s ease'
                              }} />
                            </button>
                          ) : (
                            <button 
                              className={styles.cardMapIconButton} 
                              disabled
                              title="No location data"
                            >
                              <MapIcon sx={{ 
                                fontSize: '16px', 
                                color: 'var(--Neutrals-Medium-Text)',
                                opacity: 0.5
                              }} />
                            </button>
                          )}

                          <div className={styles.destinationContent}>
                            <Link 
                              href={`/explore/${destination.destinationId}`}
                              passHref
                              style={{ textDecoration: 'none', color: 'inherit' }}
                            >
                              <div className={styles.destinationImage}>
                                <Image
                                  src={destination.coverImage || '/fallback.jpg'}
                                  alt={destination.name}
                                  fill
                                  style={{ objectFit: 'cover' }}
                                  unoptimized
                                  onError={(e) => {
                                    e.currentTarget.src = '/fallback.jpg';
                                    e.currentTarget.onerror = null;
                                  }}
                                />
                              </div>
                            </Link>
                            
                            <div className={styles.destinationInfo}>
                              <h3 className={styles.destinationName}>{destination.name}</h3>
                              
                              <div className={styles.ratingRow}>
                                <div className={styles.ratingStars}>
                                  {[1, 2, 3, 4, 5].map((star) => {
                                    const rating = destination.averageRating || 0;
                                    if (rating >= star) {
                                      return <StarIcon key={star} className={styles.starFilled} />;
                                    } else {
                                      return <StarBorderIcon key={star} className={styles.starEmpty} />;
                                    }
                                  })}
                                </div>
                                <span className={styles.reviewsCount}>
                                  {destination.reviewsCount || 0}
                                </span>
                              </div>

                              <div className={styles.metaRow}>
                                <div className={styles.destinationType}>
                                  {getTypeIcon(destination.type)}
                                  <span>{destination.type}</span>
                                </div>
                                {destination.priceRange && (
                                  <div className={styles.priceRangeTag}>
                                    <PriceRange priceRange={destination.priceRange} />
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </React.Fragment>
                ))}
              </div>
            ) : (
              <p>No destinations added yet</p>
            )}
          <div className={styles.editButtonContainer} style={{ marginTop: '16px' }}>
            <button 
              className={styles.editDestinationsButton}
              onClick={() => router.push(`/add-destinations-to-trip/${tripId}`)}
            >
              <EditIcon fontSize="small" />
              Edit Destinations
            </button>
          </div>
            </div>
        </div>

        <div className={styles.rightColumn}>
          <div className={styles.section}>
            <div className={styles.timeContainer}>
              <SameCountry sameCountry={trip?.sameCountry} />
            </div>
            <Divider sx={{ 
              height: "1px", 
              width: "100%", 
              bgcolor: "var(--Neutrals-Light-Outline)", 
              my: "16px" 
            }} />
            
            <div className={styles.statsContainer}>
              <TripStatsHydrator 
                favoritesCount={trip?.favoritesCount || 0}
                wishesCount={trip?.wishesCount || 0}
                clonesCount={trip?.clonesCount || 0}
              />
              <Stats type="Favorites" />
              <Stats type="Wishes" />
              <Stats type="Clones" />
            </div>
            
            <Divider sx={{ 
              height: "1px", 
              width: "100%", 
              bgcolor: "var(--Neutrals-Light-Outline)", 
              my: "16px" 
            }} />
            
            <h2>About</h2>
            <p className={styles.description}>{trip?.description}</p>
          </div>

          <div id="contributers" className={styles.section}>
            <h2>Contributers</h2>
            <div className={styles.contributerItem}>
              <Avatar
                alt={trip?.creator?.[1] || 'Creator'}
                src={trip?.creator?.[2]}
                sx={{ width: 56, height: 56 }}
              />
              <div className={styles.contributerInfo}>
                <span className={styles.contributerLabel}>Created by:</span>
                <span className={styles.contributerName}>
                  {trip?.creator?.[1] || 'Unknown'}
                </span>
              </div>
            </div>

            {trip?.clonedFrom && (
              <>
                <Divider sx={{ 
                  height: "1px", 
                  width: "100%", 
                  bgcolor: "var(--Neutrals-Light-Outline)", 
                  my: "16px" 
                }} />
                <h3>Inspired By</h3>
                <div className={styles.contributerItem}>
                  <Avatar
                    alt={trip.clonedFrom.creator.name}
                    src={trip.clonedFrom.creator.profileImage}
                    sx={{ width: 56, height: 56 }}
                  />
                  <div className={styles.contributerInfo}>
                    <span className={styles.contributerName}>
                      {trip.clonedFrom.creator.name}
                    </span>
                    <span className={styles.contributerLabel}>
                      Original Creator
                    </span>
                  </div>
                </div>
              </>
            )}
          </div>

          {selectedLocation && (
            <div className={styles.stickyMapContainer}>
              <LocationViewer 
                lng={selectedLocation.lng} 
                lat={selectedLocation.lat} 
                address={selectedLocation.address} 
              />
            </div>
          )}
        </div>
      </div>
    </div>
  );
}