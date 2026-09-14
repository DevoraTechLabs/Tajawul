import 'package:flutter/material.dart';
import 'package:timeline_tile/timeline_tile.dart';

class DestinationsApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: DestinationTimeline(),
      debugShowCheckedModeBanner: false,
    );
  }
}

class DestinationTimeline extends StatelessWidget {
  final List<DaySection> itinerary = [
    DaySection(
      dayLabel: 'Day 1',
      destinations: [
        Destination(
            "Brazilian Coffee", "Cafe", "Mid-Range", "assets/coffee.jpg"),
        Destination("Sultan Qaboos Grand Mosque Muscat", "Mosque", "Low",
            "assets/mosque.jpg"),
        Destination(
            "Nizwa Fort and Souq", "Fortress", "Low", "assets/fort.jpg"),
      ],
    ),
    DaySection(
      dayLabel: 'Day 2',
      destinations: [
        Destination(
            "Taiki Coffee Culture", "Cafe", "Luxury", "assets/taiki.jpg"),
        Destination("Bimmah Sinkhole - Hawiyat Najm Park", "Park", "Low",
            "assets/sinkhole.jpg"),
      ],
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Destinations')),
      body: ListView.builder(
        itemCount: itinerary.length,
        itemBuilder: (context, index) {
          final day = itinerary[index];
          return Column(
            children: [
              Padding(
                padding: const EdgeInsets.all(12.0),
                child: Chip(label: Text(day.dayLabel)),
              ),
              ...day.destinations.map((destination) {
                return TimelineTile(
                  alignment: TimelineAlign.manual,
                  lineXY: 0.1,
                  isFirst: destination == day.destinations.first,
                  isLast: destination == day.destinations.last,
                  indicatorStyle: IndicatorStyle(
                    width: 20,
                    color: Colors.teal,
                    iconStyle: IconStyle(
                        iconData: Icons.location_on, color: Colors.white),
                  ),
                  beforeLineStyle: LineStyle(color: Colors.tealAccent),
                  endChild: DestinationCard(destination: destination),
                );
              }),
            ],
          );
        },
      ),
    );
  }
}

class Destination {
  final String title;
  final String category;
  final String priceTag;
  final String imageAsset;

  Destination(this.title, this.category, this.priceTag, this.imageAsset);
}

class DaySection {
  final String dayLabel;
  final List<Destination> destinations;

  DaySection({required this.dayLabel, required this.destinations});
}

class DestinationCard extends StatelessWidget {
  final Destination destination;

  const DestinationCard({required this.destination});

  Color getPriceColor(String price) {
    switch (price.toLowerCase()) {
      case "low":
        return Colors.green;
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
        child: Row(
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(8),
              child: Image.asset(destination.imageAsset,
                  height: 70, width: 70, fit: BoxFit.cover),
            ),
            SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(destination.title,
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  SizedBox(height: 6),
                  Row(
                    children: [
                      Icon(Icons.category, size: 16, color: Colors.grey),
                      SizedBox(width: 4),
                      Text(destination.category),
                      SizedBox(width: 8),
                      Container(
                        padding:
                            EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                        decoration: BoxDecoration(
                          color: getPriceColor(destination.priceTag)
                              .withOpacity(0.2),
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: Text(
                          destination.priceTag,
                          style: TextStyle(
                              color: getPriceColor(destination.priceTag)),
                        ),
                      )
                    ],
                  ),
                ],
              ),
            ),
            Icon(Icons.map_outlined)
          ],
        ),
      ),
    );
  }
}
