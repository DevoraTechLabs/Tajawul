//Styles in Global.

import React from "react";
import { FaStar, FaStarHalfAlt, FaRegStar } from "react-icons/fa"; // Import star icons

const Rating = ({ average }) => {
  // Round to nearest 0.5
  const roundedRating = Math.round(average * 2) / 2;

  // Function to render stars based on rating
  const renderStars = () => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      if (i <= roundedRating) {
        stars.push(<FaStar key={i} className="star filled" />); // Full star
      } else if (i - 0.5 === roundedRating) {
        stars.push(<FaStarHalfAlt key={i} className="star half" />); // Half star
      } else {
        stars.push(<FaRegStar key={i} className="star empty" />); // Empty star
      }
    }
    return stars;
  };

  return <div className="rating">{renderStars()}</div>;
};

export default Rating;
