import 'package:flutter/material.dart';
import 'package:timeline_tile/timeline_tile.dart';
import 'package:tajawul/trip.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class TripDetailsPage extends StatefulWidget {
  final Trip trip;

  const TripDetailsPage({super.key, required this.trip});

  @override
  State<TripDetailsPage> createState() => _TripDetailsPageState();
}

class _TripDetailsPageState extends State<TripDetailsPage> {
  List<ApiDestination> destinations = [];
  bool isLoadingDestinations = true;
  String errorMessage = '';

  @override
  void initState() {
    super.initState();
    fetchDestinations();
  }

  Future<void> fetchDestinations() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('auth_token') ?? '';

    if (token.isEmpty) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
              content: Text("Authentication required. Please login.")),
        );
      }
      setState(() {
        isLoadingDestinations = false;
        errorMessage = 'Authentication required';
      });
      return;
    }

    final url =
        'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Trip/${widget.trip.tripId}/destinations';

    try {
      final response = await http.get(
        Uri.parse(url),
        headers: {
          'accept': '*/*',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = json.decode(response.body);
        setState(() {
          destinations =
              data.map((json) => ApiDestination.fromJson(json)).toList();
          isLoadingDestinations = false;
          errorMessage = '';
        });
      } else {
        setState(() {
          isLoadingDestinations = false;
          errorMessage = 'Failed to load destinations: ${response.statusCode}';
        });
      }
    } catch (e) {
      setState(() {
        isLoadingDestinations = false;
        errorMessage = 'Error fetching destinations: ${e.toString()}';
      });
    }
  }

  String _getPriceLabel(PriceLevel level) {
    switch (level) {
      case PriceLevel.low:
        return '\$ Low';
      case PriceLevel.midRange:
        return '\$\$ Mid-Range';
      case PriceLevel.luxury:
        return '\$\$\$ Luxury';
    }
  }

  Color _getPriceColor(PriceLevel level) {
    switch (level) {
      case PriceLevel.low:
        return Colors.green;
      case PriceLevel.midRange:
        return Colors.orange;
      case PriceLevel.luxury:
        return Colors.amber.shade700;
    }
  }

  List<DaySection> _groupDestinationsByDay() {
    final dayGroups = <int, List<ApiDestination>>{};

    for (final dest in destinations) {
      dayGroups.putIfAbsent(dest.day, () => []).add(dest);
    }

    return dayGroups.entries.map((entry) {
      return DaySection(
        dayLabel: 'Day ${entry.key}',
        destinations: entry.value
            .map((apiDest) => Destination(
                  apiDest.name,
                  apiDest.type,
                  apiDest.priceRange,
                  apiDest.coverImage,
                  apiDest.city,
                  apiDest.country,
                  apiDest.locations.firstOrNull?.address ?? '',
                ))
            .toList(),
      );
    }).toList()
      ..sort((a, b) => a.dayLabel.compareTo(b.dayLabel));
  }

  @override
  Widget build(BuildContext context) {
    final itinerary = isLoadingDestinations ? [] : _groupDestinationsByDay();

    return Scaffold(
      body: CustomScrollView(
        slivers: [
          SliverAppBar(
            expandedHeight: 220,
            pinned: true,
            backgroundColor: Colors.brown,
            flexibleSpace: FlexibleSpaceBar(
              title: Text(
                widget.trip.title,
                style: TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 12, // Adjust size as needed
                ),
              ),
              background: widget.trip.imageUrl.isNotEmpty
                  ? Image.network(widget.trip.imageUrl, fit: BoxFit.cover)
                  : Container(color: Colors.grey[300]),
            ),
            actions: [
              IconButton(icon: const Icon(Icons.camera_alt), onPressed: () {}),
              IconButton(
                  icon: const Icon(Icons.favorite_border), onPressed: () {}),
              IconButton(
                  icon: const Icon(Icons.bookmark_border), onPressed: () {}),
              IconButton(icon: const Icon(Icons.copy), onPressed: () {}),
            ],
          ),
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Wrap(spacing: 8, children: [
                      _buildTag(_getPriceLabel(widget.trip.priceLevel),
                          _getPriceColor(widget.trip.priceLevel)),
                      _buildTag("Short", Colors.amber),
                      _buildTag("Not Started", Colors.brown),
                    ]),
                    const SizedBox(height: 8),
                    Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text("No tags added yet"),
                          ElevatedButton(
                            onPressed: () {},
                            style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.black),
                            child: const Text("Edit Tags",
                                style: TextStyle(color: Colors.white)),
                          ),
                        ]),
                    const Divider(height: 32),
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: Colors.grey[100],
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text("🌍 Same Country",
                                style: TextStyle(fontWeight: FontWeight.bold)),
                            const SizedBox(height: 12),
                            Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceEvenly,
                                children: const [
                                  Column(children: [
                                    Icon(Icons.favorite_border),
                                    SizedBox(height: 4),
                                    Text("0\nFavorites",
                                        textAlign: TextAlign.center)
                                  ]),
                                  Column(children: [
                                    Icon(Icons.favorite),
                                    SizedBox(height: 4),
                                    Text("0\nWishes",
                                        textAlign: TextAlign.center)
                                  ]),
                                ]),
                          ]),
                    ),
                    const SizedBox(height: 32),
                    const Text("Trip Highlights",
                        style: TextStyle(
                            fontSize: 20, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 12),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.network(
                        'https://picsum.photos/600/300',
                        height: 200,
                        width: double.infinity,
                        fit: BoxFit.cover,
                      ),
                    ),
                    const SizedBox(height: 16),
                    const Text("About",
                        style: TextStyle(
                            fontSize: 18, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 8),
                    Text(
                        "Discover ${widget.trip.title}\n\nLorem ipsum dolor sit amet, consectetur adipiscing elit. "
                        "Sed non risus. Suspendisse lectus tortor, dignissim sit amet."),
                    const SizedBox(height: 24),
                    const Text("Itinerary Timeline",
                        style: TextStyle(
                            fontSize: 18, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 8),
                    if (isLoadingDestinations)
                      const Center(child: CircularProgressIndicator())
                    else if (errorMessage.isNotEmpty)
                      Text(errorMessage,
                          style: const TextStyle(color: Colors.red))
                    else if (itinerary.isEmpty)
                      const Text("No destinations added yet")
                    else
                      ...itinerary.map((day) => Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Padding(
                                padding:
                                    const EdgeInsets.symmetric(vertical: 8),
                                child: Chip(label: Text(day.dayLabel)),
                              ),
                              ...day.destinations.map((destination) {
                                return TimelineTile(
                                  alignment: TimelineAlign.manual,
                                  lineXY: 0.1,
                                  isFirst:
                                      destination == day.destinations.first,
                                  isLast: destination == day.destinations.last,
                                  indicatorStyle: IndicatorStyle(
                                    width: 20,
                                    color: Colors.teal,
                                    iconStyle: IconStyle(
                                        iconData: Icons.location_on,
                                        color: Colors.white),
                                  ),
                                  beforeLineStyle:
                                      LineStyle(color: Colors.tealAccent),
                                  endChild:
                                      DestinationCard(destination: destination),
                                );
                              }),
                            ],
                          )),
                    const SizedBox(height: 24),
                    const Text("Contributors",
                        style: TextStyle(
                            fontSize: 18, fontWeight: FontWeight.bold)),
                    const ListTile(
                      leading: CircleAvatar(
                          backgroundImage:
                              NetworkImage("https://i.pravatar.cc/100")),
                      title: Text("Created by:"),
                      subtitle: Text("Shahd Gamal"),
                    ),
                  ]),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTag(String label, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration:
          BoxDecoration(color: color, borderRadius: BorderRadius.circular(10)),
      child: Text(label,
          style: const TextStyle(color: Colors.white, fontSize: 12)),
    );
  }
}

class Destination {
  final String title;
  final String category;
  final String priceTag;
  final String imageUrl;
  final String city;
  final String country;
  final String address;

  Destination(
    this.title,
    this.category,
    this.priceTag,
    this.imageUrl,
    this.city,
    this.country,
    this.address,
  );
}

class DaySection {
  final String dayLabel;
  final List<Destination> destinations;

  DaySection({required this.dayLabel, required this.destinations});
}

class DestinationCard extends StatelessWidget {
  final Destination destination;

  const DestinationCard({super.key, required this.destination});

  Color getPriceColor(String price) {
    switch (price.toLowerCase()) {
      case "low":
        return Colors.green;
      case "mid":
      case "mid-range":
        return Colors.orange;
      case "luxury":
        return Colors.black87;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Row(children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: destination.imageUrl.isNotEmpty
                ? Image.network(
                    destination.imageUrl,
                    height: 70,
                    width: 70,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      height: 70,
                      width: 70,
                      color: Colors.grey[300],
                      child: const Icon(Icons.image),
                    ),
                  )
                : Container(
                    height: 70,
                    width: 70,
                    color: Colors.grey[300],
                    child: const Icon(Icons.image),
                  ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(destination.title,
                    style: const TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 4),
                Text('${destination.city}, ${destination.country}',
                    style: const TextStyle(fontSize: 12, color: Colors.grey)),
                const SizedBox(height: 6),
                Row(children: [
                  const Icon(Icons.category, size: 16, color: Colors.grey),
                  const SizedBox(width: 4),
                  Text(destination.category),
                  const SizedBox(width: 8),
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color:
                          getPriceColor(destination.priceTag).withOpacity(0.2),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      destination.priceTag,
                      style:
                          TextStyle(color: getPriceColor(destination.priceTag)),
                    ),
                  ),
                ]),
                if (destination.address.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    destination.address,
                    style: const TextStyle(fontSize: 11, color: Colors.grey),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ],
            ),
          ),
          const Icon(Icons.chevron_right, color: Colors.grey),
        ]),
      ),
    );
  }
}
