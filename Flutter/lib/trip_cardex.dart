import 'package:flutter/material.dart';
import 'package:tajawul/trip.dart';
import 'package:tajawul/trip_details_page.dart';

class TripCard extends StatelessWidget {
  final Trip trip;

  const TripCard({super.key, required this.trip});

  Color _getPriceColor() {
    switch (trip.priceLevel) {
      case PriceLevel.low:
        return Colors.green;
      case PriceLevel.midRange:
        return Colors.orange;
      case PriceLevel.luxury:
        return Colors.amber.shade700;
    }
  }

  String _getPriceLabel() {
    switch (trip.priceLevel) {
      case PriceLevel.low:
        return '\$ Low';
      case PriceLevel.midRange:
        return '\$\$ Mid-Range';
      case PriceLevel.luxury:
        return '\$\$\$ Luxury';
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
        onTap: () {
          Navigator.push(
            context,
            MaterialPageRoute(builder: (_) => TripDetailsPage(trip: trip)),
          );
        },
        child: Card(
          elevation: 4,
          margin: const EdgeInsets.all(8),
          shape:
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Stack(
                children: [
                  ClipRRect(
                    borderRadius:
                        const BorderRadius.vertical(top: Radius.circular(16)),
                    child: trip.imageUrl.isEmpty
                        ? Container(
                            height: 120,
                            color: Colors.grey[300],
                            child: const Center(
                                child: Icon(Icons.image, size: 40)),
                          )
                        : Image.network(
                            trip.imageUrl,
                            height: 120,
                            width: double.infinity,
                            fit: BoxFit.cover,
                          ),
                  ),
                  Positioned(
                    top: 8,
                    left: 8,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 4),
                      decoration: BoxDecoration(
                        color: _getPriceColor(),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        _getPriceLabel(),
                        style:
                            const TextStyle(color: Colors.white, fontSize: 12),
                      ),
                    ),
                  ),
                  const Positioned(
                    top: 8,
                    right: 8,
                    child: Icon(Icons.bookmark_border, color: Colors.white),
                  ),
                ],
              ),
              Padding(
                padding: const EdgeInsets.all(8),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(trip.title,
                        style: const TextStyle(
                            fontWeight: FontWeight.bold, fontSize: 16)),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        const Icon(Icons.access_time, size: 14),
                        const SizedBox(width: 4),
                        Text(trip.duration,
                            style: const TextStyle(fontSize: 12)),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        const Icon(Icons.public, size: 14),
                        const SizedBox(width: 4),
                        Text(trip.countryType,
                            style: const TextStyle(fontSize: 12)),
                      ],
                    ),
                  ],
                ),
              )
            ],
          ),
        ));
  }
}
