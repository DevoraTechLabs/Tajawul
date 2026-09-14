import 'package:flutter/material.dart';
import 'package:tajawul/trip_card.dart';

class TripCarousel extends StatelessWidget {
  final List<String> tripImages = [
    'assets/image.png',
    'assets/image.png',
    'assets/image.png',
  ];

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 400, // Adjust height as needed
      child: PageView.builder(
        itemCount: 3, // Number of trip cards
        controller: PageController(viewportFraction: 0.8), // Controls card size
        itemBuilder: (context, index) {
          return Transform.scale(
            scale: index == 1 ? 1.0 : 0.9, // Highlight center card
            // ignore: prefer_const_constructors
            child: TripCard(imagePath: tripImages[index]),
          );
        },
      ),
    );
  }
}
