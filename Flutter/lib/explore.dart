import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import 'package:tajawul/distnation.dart';
import 'package:tajawul/models/destination.dart';
import 'dart:convert';
import 'custom_drawer.dart';

class ExploreScreen extends StatelessWidget {
  final DestinationService destinationService = DestinationService();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const CustomDrawer(),
      appBar: AppBar(
        title: const Text("Explore"),
        backgroundColor: Colors.brown,
        leading: Builder(
          builder: (context) => IconButton(
            icon: const Icon(Icons.menu, color: Colors.white),
            onPressed: () => Scaffold.of(context).openDrawer(),
          ),
        ),
      ),
      body: SafeArea(
        child: FutureBuilder<DestinationResponse>(
          future: destinationService.getDestinations(),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            } else if (snapshot.hasError) {
              return Center(child: Text('Error: ${snapshot.error}'));
            } else if (!snapshot.hasData ||
                snapshot.data!.destinations.isEmpty) {
              return const Center(child: Text('No destinations found'));
            } else {
              return SingleChildScrollView(
                child: Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: PlacesGrid(destinations: snapshot.data!.destinations),
                ),
              );
            }
          },
        ),
      ),
    );
  }
}

class PlacesGrid extends StatelessWidget {
  final List<Destination> destinations;

  const PlacesGrid({required this.destinations, Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(
      children: destinations.map((destination) {
        return Padding(
          padding: const EdgeInsets.symmetric(vertical: 10.0),
          child: CustomCard(destination: destination),
        );
      }).toList(),
    );
  }
}

class CustomCard extends StatelessWidget {
  final Destination destination;

  const CustomCard({required this.destination, Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => DestinationScreen(destination: destination),
          ),
        );
      },
      child: Card(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
        elevation: 4,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                // Image with loading and error handling
                ClipRRect(
                  borderRadius:
                      const BorderRadius.vertical(top: Radius.circular(15)),
                  child: Image.network(
                    destination.coverImage,
                    width: double.infinity,
                    height: 180,
                    fit: BoxFit.cover,
                    loadingBuilder: (context, child, loadingProgress) {
                      if (loadingProgress == null) return child;
                      return Container(
                        height: 180,
                        color: Colors.grey[300],
                        child: const Center(child: CircularProgressIndicator()),
                      );
                    },
                    errorBuilder: (context, error, stackTrace) {
                      return Container(
                        height: 180,
                        color: Colors.grey[300],
                        child: const Center(
                          child: Icon(Icons.image_not_supported,
                              size: 50, color: Colors.grey),
                        ),
                      );
                    },
                  ),
                ),
                // Favorite button
                Positioned(
                  top: 10,
                  right: 10,
                  child: Container(
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: Colors.black.withOpacity(0.5),
                    ),
                    child: IconButton(
                      icon: const Icon(Icons.favorite_border,
                          color: Colors.white),
                      onPressed: () {},
                    ),
                  ),
                ),
                // Location info
                Positioned(
                  bottom: 10,
                  left: 10,
                  child: Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                    decoration: BoxDecoration(
                      color: Colors.black.withOpacity(0.7),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.location_on,
                            size: 16, color: Colors.white),
                        const SizedBox(width: 5),
                        Text(
                          '${destination.city}, ${destination.country}',
                          style: const TextStyle(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
            // Destination info
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    destination.name,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    destination.type,
                    style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                  ),
                  const SizedBox(height: 5),
                  Row(
                    children: List.generate(
                      5,
                      (index) => Icon(
                        index < destination.averageRating.round()
                            ? Icons.star
                            : Icons.star_border,
                        color: Colors.teal,
                        size: 18,
                      ),
                    ),
                  ),
                  const SizedBox(height: 5),
                  Text(
                    'Price: ${destination.priceRange}',
                    style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// Your existing Destination, DestinationResponse, and DestinationService classes remain the same

class DestinationResponse {
  final int count;
  final List<Destination> destinations;

  DestinationResponse({
    required this.count,
    required this.destinations,
  });

  factory DestinationResponse.fromJson(Map<String, dynamic> json) {
    return DestinationResponse(
      count: json['count'] ?? 0,
      destinations: (json['destinations'] as List?)
              ?.map((dest) => Destination.fromJson(dest))
              .toList() ??
          [],
    );
  }
}

class DestinationService {
  static const String baseUrl =
      'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api';

  Future<DestinationResponse> getDestinations() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('auth_token') ?? '';

      if (token.isEmpty) {
        throw Exception('Authentication required. Please login.');
      }

      final response = await http.get(
        Uri.parse('$baseUrl/Destination'),
        headers: {
          'accept': 'application/json',
          'Authorization': 'Bearer $token',
        },
      ).timeout(const Duration(seconds: 30));

      if (response.statusCode == 200) {
        try {
          final jsonResponse = json.decode(response.body);
          return DestinationResponse.fromJson(jsonResponse);
        } catch (e) {
          throw FormatException('Failed to parse JSON: $e');
        }
      } else {
        throw Exception('Failed to load destinations: ${response.statusCode}');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}');
    } catch (e) {
      throw Exception('Error: $e');
    }
  }
}
