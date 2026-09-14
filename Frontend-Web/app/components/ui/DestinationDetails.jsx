import axios from "axios";
import Image from "next/image";
import Link from "next/link";
import VerifiedIcon from "@mui/icons-material/Verified";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import Divider from "@mui/material/Divider";
import Avatar from "@mui/material/Avatar";
import Editors from "@/components/ui/Editors";
import Rating from "@/components/ui/Rating";
import OpenClose from "@/components/ui/OpenClose";
import Stats from "@/components/ui/Stats";
import ImageList from "@/components/ui/ImageList";
import styles from "@/destination.module.css";
import PriceRange from "./tags/PriceRange";
import EditTags from "@/components/ui/edit/EditTags";
import WriteReview from "@/components/ui/edit/WriteReview";
import GroupSize from "./tags/GroupSize";
import Tag from "./tags/Tag";
import DestinationIdHandler from "@/components/DestinationIdHandler";
import DestinationInteractions from "./DestinationInteractions";
import StatsHydrator from "@/components/StatsHydrator";
import PhoneIcon from "@mui/icons-material/Phone";
import FacebookIcon from "@mui/icons-material/Facebook";
import XIcon from "@mui/icons-material/X";
import InstagramIcon from "@mui/icons-material/Instagram";
import LinkedInIcon from "@mui/icons-material/LinkedIn";
import LanguageIcon from "@mui/icons-material/Language";
import Review from "@/components/ui/Review";

export default async function DestinationDetails({ destinationId }) {
  await new Promise((resolve) => setTimeout(resolve, 2000));

  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  let destination = null;
  let groupSizes = [];
  let activities = [];
  let tags = [];
  let contributersData = { users: [] };

  try {
    const { data: destinationData } = await axios.get(
      `${baseUrl}/Destination?DestinationId=${destinationId}`
    );

    const { data: attributesData } = await axios.get(
      `${baseUrl}/Destination/${destinationId}/attributes`
    );

    const { data: contributersResponse } = await axios.get(
      `${baseUrl}/Destination/${destinationId}/users?Relation=contribute`
    );
    contributersData = contributersResponse || { users: [] };
    console.log("Raw attributes response:", attributesData);

    const { data: reviewsData } = await axios.get(
      `${baseUrl}/Review?DestinationId=${destinationId}`
    );

    destination = {
      ...destinationData.destinations[0],
      reviews: reviewsData.reviews,
    };

    // Extract just the names from each attribute
    groupSizes =
      attributesData.groupSizes?.data?.map((item) => item.group) || [];
    activities =
      attributesData.activities?.data?.map((item) => item.name) || [];
    tags = attributesData.tags?.data?.map((item) => item.name) || [];

    if (!destination) throw new Error("Destination not found");

    console.log("Processed Destination Details:", {
      destination,
      attributes: {
        groupSizes,
        activities,
        tags,
      },
    });
  } catch (error) {
    console.error("Failed to fetch destination details:", error);
    contributersData = { users: [] };
  }

  return (
    <div>
      <DestinationIdHandler destinationId={destinationId} />
      <div className={styles.coverWrapper}>
        <div className={styles.destinationCover}>
          {destination?.coverImage && (
            <Image
              src={destination.coverImage}
              alt={`Destination is ${destination.name}`}
              fill={true}
              style={{ objectFit: "cover" }}
              priority
            />
          )}
        </div>
        {/* Content Above the Image */}
        <div className={styles.coverContent}>
          {/* Breadcrumb */}
          <div className={styles.breadcrumb}>
            <p>
              <span>All</span>
              <span className={styles.separator}>&gt;</span>
              <span>{destination?.country}</span>
              <span className={styles.separator}>&gt;</span>
              <span>{destination?.city}</span>
              <span className={styles.separator}>&gt;</span>
              <span>{destination?.name}</span>
            </p>
          </div>
          <div className={styles.buttomContainer}>
            <div className={styles.buttomLeftContainer}>
              {/* Is Verified */}
              {destination?.isVerified && (
                <div className={styles.verified}>
                  <VerifiedIcon
                    sx={{
                      fontSize: "20px",
                      color: "var(--Neutrals-Background)",
                    }}
                  />
                  <p>Verified Destination</p>
                </div>
              )}
              {/* Destination Name */}
              <h1 className={styles.destinationName}>{destination?.name}</h1>
              {/* Rating and Reviews Count */}
              <div className={styles.ratingContainer}>
                <Rating average={destination?.averageRating} />
                <p className={styles.reviewsCount}>
                  {destination?.reviewsCount} Reviews
                </p>
              </div>
            </div>
            <div className={styles.buttomRightContainer}>
              <DestinationInteractions destinationId={destinationId} />
            </div>
          </div>
        </div>
      </div>
      {/* Secondary Nav Bar */}
      <div className={styles.secondaryNavBar}>
        <div className={styles.navContainer}>
          <a href="#reviews" className={styles.navItem}>
            Reviews
          </a>
          <a href="#location" className={styles.navItem}>
            Location
          </a>
          <a href="#contributers" className={styles.navItem}>
            Contributers
          </a>
        </div>
        <div className={styles.contactContainer}>
          {/* Website */}
          {destination?.contactInfo?.find((c) => c.type === "Website") && (
            <Link
              href={
                destination.contactInfo.find((c) => c.type === "Website").value
              }
              target="_blank"
              rel="noopener noreferrer"
              passHref
            >
              <LanguageIcon />
            </Link>
          )}

          {/* Social Media */}
          {destination?.socialMediaLinks?.map((social) => {
            const platform = social.platform.toLowerCase();
            return (
              <Link
                key={social.url}
                href={social.url}
                target="_blank"
                rel="noopener noreferrer"
                passHref
              >
                {platform.includes("facebook") && <FacebookIcon />}
                {(platform.includes("twitter") || platform.includes("x")) && (
                  <XIcon />
                )}
                {(platform.includes("instgram") ||
                  platform.includes("instagram")) && <InstagramIcon />}
                {platform.includes("linkedin") && <LinkedInIcon />}
              </Link>
            );
          })}

          {/* Phone - Note: Link doesn't work well with tel: links, so we'll keep it as <a> */}
          {destination?.contactInfo?.find((c) => c.type === "Phone") && (
            <a
              href={`tel:${
                destination.contactInfo.find((c) => c.type === "Phone").value
              }`}
            >
              <PhoneIcon />
              <span>
                {destination.contactInfo.find((c) => c.type === "Phone").value}
              </span>
            </a>
          )}
        </div>
      </div>
      <div className={styles.mainContent}>
        {/* Left Column */}
        <div className={styles.leftColumn}>
          <div
            id="tags"
            className={`${styles.section} ${styles.tagsContainer}`}
          >
            <div className={styles.headerRow}>
              <div className={styles.tagsWrapper}>
                <PriceRange priceRange={destination?.priceRange} />
                <GroupSize groupSizes={groupSizes} />
                {/* Pass the array directly */}
              </div>
              <Divider
                sx={{
                  height: "1px",
                  width: "100%",
                  bgcolor: "var(--Neutrals-Light-Outline)",
                  mb: "8px",
                }}
              />
              {/* Updated Tags Section */}
              {(() => {
                const hasTags = tags.length > 0;
                const hasActivities = activities.length > 0;
                const hasContent = hasTags || hasActivities;

                return (
                  <>
                    {hasContent ? (
                      <div className={styles.tagsWrapper}>
                        <Tag options={tags} />
                        <Tag options={activities} />
                      </div>
                    ) : (
                      <div className={styles.noTagsMessage}>
                        No Tags or Activities are added, yet.
                      </div>
                    )}
                  </>
                );
              })()}
              <div className={styles.editButtonContainer}>
                <EditTags destinationId={destinationId} />
              </div>
            </div>
          </div>
          <div id="images" className={`${styles.section} ${styles.images}`}>
            <ImageList images={destination?.images} />
          </div>
          <div id="reviews" className={`${styles.section} ${styles.reviews}`}>
            <div className={styles.reviewsHeader}>
              <h2>Reviews</h2>
              <WriteReview destinationId={destinationId} />
            </div>

            <div className={styles.reviewsList}>
              {destination?.reviews?.length > 0 ? (
                destination.reviews.map((review) => (
                  <Review key={review.reviewId} review={review} />
                ))
              ) : (
                <p className={styles.noReviews}>
                  No reviews yet. Be the first to review!
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Right Column */}
        <div className={styles.rightColumn}>
          <div id="about" className={`${styles.section} ${styles.about}`}>
            {/* Open/Close Time */}
            <OpenClose
              isOpen24Hours={destination?.isOpen24Hours}
              openTime={destination?.openTime}
              closeTime={destination?.closeTime}
            />
            <Divider
              sx={{
                height: "1px",
                width: "100%",
                bgcolor: "var(--Neutrals-Light-Outline)",
              }}
            />
            {/* Destination Stats */}
            <div className={styles.statsContainer}>
              {/* 🧪 Inject initializer component with server-side data */}
              <StatsHydrator
                wishesCount={destination?.wishesCount || 0}
                visitorsCount={destination?.visitorsCount || 0}
                followersCount={destination?.followersCount || 0}
              />

              <Stats type="Wishes" />
              <Stats type="Visitors" />
              <Stats type="Followers" />
            </div>
            <Divider
              sx={{
                height: "1px",
                width: "100%",
                bgcolor: "var(--Neutrals-Light-Outline)",
              }}
            />
            {/* About */}
            <h2>About</h2>
            <div>
              <p className={styles.type}>{destination?.type}</p>
              <p className={styles.description}>{destination?.description}</p>
            </div>

            {destination?.establishedAt && (
              <div
                style={{
                  width: "100%",
                  display: "flex",
                  flexDirection: "column",
                }}
              >
                <Divider
                  sx={{
                    height: "1px",
                    width: "100%",
                    bgcolor: "var(--Neutrals-Light-Outline)",
                    mb: "16px",
                  }}
                />
                <span style={{ width: "100%", textAlign: "left" }}>
                  <strong className={styles.important}>Established At:</strong>{" "}
                  {new Date(destination.establishedAt).getFullYear()}
                </span>
              </div>
            )}
          </div>
          <div
            id="contributers"
            className={`${styles.section} ${styles.contributers}`}
          >
            <h2>Contributers</h2>
            <div className={styles.creator}>
              <Avatar
                alt={destination?.creator[1]}
                src={destination?.creator[2]}
                sx={{ width: 56, height: 56 }}
              />
              <span className={styles.important}>Created By:</span>
              <span>{destination?.creator[1]}</span>
            </div>
            <Divider
              sx={{
                height: "1px",
                width: "100%",
                bgcolor: "var(--Neutrals-Light-Outline)",
              }}
            />
            <Editors
              editors={
                contributersData?.users?.map((user) => ({
                  id: user.id,
                  name: user.name,
                  url: user.image, // Changed from profileImage to image
                })) || []
              }
            />
          </div>
          <div id="location" className={`${styles.section} ${styles.location}`}>
            <h2>Location</h2>
            <div className={styles.locationContainer}>
              <LocationOnIcon sx={{ mt: "20px" }} />
              <p>
                {destination?.locations?.[0]?.address ||
                  "Location not available"}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
