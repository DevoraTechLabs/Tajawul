import 'package:flutter/material.dart';
import 'package:carousel_slider/carousel_slider.dart';
import 'package:carousel_slider/carousel_controller.dart' as carousel;

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: DestinationDetailsScreen(),
    );
  }
}

class DestinationDetailsScreen extends StatelessWidget {
  final carousel.CarouselController _controller = carousel.CarouselController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Destination Details')),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                Container(
                  width: double.infinity,
                  height: 250,
                  color: Colors.grey, // Placeholder for image
                ),
                Positioned(
                  top: 40,
                  left: 16,
                  child: BackButton(color: Colors.white),
                ),
                Positioned(
                  top: 40,
                  right: 16,
                  child: Icon(Icons.favorite_border, color: Colors.white),
                ),
                Positioned(
                  bottom: 16,
                  left: 16,
                  child: Text(
                    'Destination Name',
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                ),
              ],
            ),
            Padding(
              padding: const EdgeInsets.all(16.0),
              child: Wrap(
                spacing: 8.0,
                children: [
                  _buildTag('Mid-Range'),
                  _buildTag('Nature & Adventure'),
                  _buildTag('Family'),
                  _buildTag('Friends'),
                  _buildTag('History & Culture'),
                ],
              ),
            ),
            CarouselSlider(
              options: CarouselOptions(height: 200.0, autoPlay: true),
              items: List.generate(3, (index) {
                return Container(
                  margin: EdgeInsets.symmetric(horizontal: 5.0),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(10.0),
                    color: Colors.grey, // Placeholder for images
                  ),
                );
              }),
              carouselController: _controller,
            ),
            Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'About',
                    style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 8),
                  Text('Destination description goes here...'),
                  SizedBox(height: 16),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    children: [
                      _buildStat('89K', 'Followers'),
                      _buildStat('30K', 'Visitors'),
                      _buildStat('250', 'Events'),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTag(String tag) {
    return Chip(
      label: Text(tag, style: TextStyle(color: Colors.white)),
      backgroundColor: Colors.brown,
    );
  }

  Widget _buildStat(String value, String label) {
    return Column(
      children: [
        Text(value, style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
        Text(label),
      ],
    );
  }
}
