import 'package:flutter/material.dart';
import 'package:tajawul/custom_drawer.dart';
import 'package:tajawul/nav_bar.dart';
import 'package:tajawul/models/destination.dart'; // Assuming you have this model

class DestinationScreen extends StatelessWidget {
  final Destination destination;

  const DestinationScreen({required this.destination, Key? key})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          return SingleChildScrollView(
            child: Column(
              children: [
                // Header Image Section
                SizedBox(
                  height: constraints.maxHeight * 0.6,
                  child: Stack(
                    children: [
                      // Main Image
                      Positioned.fill(
                        child: Image.network(
                          destination.coverImage,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) =>
                              Container(color: Colors.grey[300]),
                        ),
                      ),
                      // Gradient Overlay
                      Positioned.fill(
                        child: Container(
                          decoration: BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.bottomCenter,
                              end: Alignment.topCenter,
                              colors: [
                                Colors.black.withOpacity(0.7),
                                Colors.transparent,
                              ],
                            ),
                          ),
                        ),
                      ),
                      // Nav Bar
                      const Positioned(
                        top: 0,
                        left: 0,
                        right: 0,
                        child: NavBar(),
                      ),
                      // Destination Info
                      Positioned(
                        bottom: 20,
                        left: 20,
                        right: 20,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            if (destination.isVerified)
                              Text(
                                "Verified Destination",
                                style: TextStyle(
                                  color: Colors.white70,
                                  fontSize: constraints.maxWidth * 0.04,
                                ),
                              ),
                            const SizedBox(height: 8),
                            Text(
                              destination.name,
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: constraints.maxWidth * 0.08,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 16),
                            Row(
                              children: [
                                Icon(Icons.star, color: Colors.yellow),
                                const SizedBox(width: 8),
                                Text(
                                  "${destination.averageRating.toStringAsFixed(1)} (${destination.visitorsCount} reviews)",
                                  style: TextStyle(
                                    color: Colors.white70,
                                    fontSize: constraints.maxWidth * 0.04,
                                  ),
                                ),
                                const Spacer(),
                                ElevatedButton.icon(
                                  onPressed: () {},
                                  icon: const Icon(Icons.favorite_border),
                                  label: const Text("Save"),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor:
                                        Colors.white.withOpacity(0.2),
                                    foregroundColor: Colors.white,
                                    shape: RoundedRectangleBorder(
                                      borderRadius: BorderRadius.circular(30),
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
                // Content Section
                DestinationContent(destination: destination),
              ],
            ),
          );
        },
      ),
    );
  }
}

class DestinationContent extends StatelessWidget {
  final Destination destination;

  const DestinationContent({required this.destination, Key? key})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Tags/Categories
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: [
              Chip(label: Text(destination.type)),
              Chip(label: Text(destination.priceRange)),
              if (destination.isOpen24Hours)
                const Chip(label: Text("24 Hours")),
            ],
          ),
          const SizedBox(height: 20),

          // Image Gallery
          if (destination.images.isNotEmpty)
            SizedBox(
              height: 200,
              child: PageView.builder(
                itemCount: destination.images.length,
                itemBuilder: (context, index) {
                  return Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 8.0),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.network(
                        destination.images[index],
                        fit: BoxFit.cover,
                        errorBuilder: (context, error, stackTrace) =>
                            Container(color: Colors.grey[300]),
                      ),
                    ),
                  );
                },
              ),
            ),
          const SizedBox(height: 20),

          // Stats
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceAround,
            children: [
              _buildStatItem("Visitors", destination.visitorsCount),
            ],
          ),
          const SizedBox(height: 20),

          // About Section
          const Text(
            "About",
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 8),
          Text(
            destination.description,
            style: TextStyle(color: Colors.grey[700]),
          ),
          const SizedBox(height: 20),

          // Location Info
          if (destination.locations.isNotEmpty) ...[
            const Text(
              "Location",
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Text(
              destination.locations.first.address,
              style: TextStyle(color: Colors.grey[700]),
            ),
            const SizedBox(height: 20),
          ],

          // Action Buttons
          // Row(
          //   mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          //   children: [
          //     ElevatedButton(
          //       onPressed: () {},
          //       child: const Text("Write Review"),
          //     ),
          //     ElevatedButton(
          //       onPressed: () {},
          //       child: const Text("Share"),
          //     ),
          //   ],
          // ),
        ],
      ),
    );
  }

  Widget _buildStatItem(String label, int count) {
    return Column(
      children: [
        Text(
          count.toString(),
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        Text(
          label,
          style: TextStyle(color: Colors.grey[600]),
        ),
      ],
    );
  }
}
