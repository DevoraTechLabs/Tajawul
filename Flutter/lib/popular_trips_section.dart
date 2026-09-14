import 'package:flutter/material.dart';
import 'package:tajawul/trip_carousel.dart';

class PopularTripsSection extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;

    return Padding(
      padding: EdgeInsets.symmetric(
        vertical: screenWidth * 0.08,
        horizontal: screenWidth * 0.05,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            'Most Popular Trips This Week',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Color(0xFF4A2D1F),
            ),
          ),
          SizedBox(height: 10),
          Text(
            'Browse unique trip templates or share your own adventures on our TripHub',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 16,
              color: Color(0xFF6A4F40),
            ),
          ),
          SizedBox(height: 20),
          SizedBox(
            height: 350,
            child: TripCarousel(),
          ),
        ],
      ),
    );
  }
}
