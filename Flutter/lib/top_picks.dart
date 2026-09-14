import 'package:flutter/material.dart';
import 'package:flutter_staggered_grid_view/flutter_staggered_grid_view.dart';

class TopPicks extends StatelessWidget {
  final List<Map<String, dynamic>> picks = [
    {
      'title': 'Top Saved Destination',
      'subtitle': 'Destination Name',
      'image': 'assets/image.png',
      'likes': 2467,
      'large': true, // Large item
    },
    {
      'title': 'Café',
      'subtitle': 'Top Category',
      'image': 'assets/image.png',
      'likes': 2467,
      'large': false,
    },
    {
      'title': 'Top Hotel',
      'subtitle': 'Hotel Name',
      'image': 'assets/image.png',
      'likes': 2467,
      'large': true, // Large item
    },
    {
      'title': 'Top Restaurant',
      'subtitle': 'Hotel Name',
      'image': 'assets/image.png',
      'likes': 2467,
      'large': false,
    },
    {
      'title': 'Night Tours',
      'subtitle': 'Top Thing To Do',
      'image': 'assets/image.png',
      'likes': 568,
      'large': false,
    },
  ];

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        children: [
          Text(
            "This Month’s Top Picks",
            textAlign: TextAlign.center,
            style: TextStyle(
                fontSize: 24, fontWeight: FontWeight.bold, color: Colors.brown),
          ),
          SizedBox(height: 8),
          Text(
            "Explore the hottest destinations, categories, and experiences this month—updated just for you!",
            style: TextStyle(color: Colors.grey[600]),
            textAlign: TextAlign.center,
          ),
          SizedBox(height: 16),
          StaggeredGrid.count(
            crossAxisCount: 2,
            mainAxisSpacing: 8,
            crossAxisSpacing: 8,
            children: picks.map((pick) {
              return StaggeredGridTile.fit(
                crossAxisCellCount: pick['large'] ? 3 : 1,
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: Stack(
                    children: [
                      Image.asset(
                        pick['image'],
                        fit: BoxFit.cover,
                        width: double.infinity,
                        height: pick['large'] ? 150 : 100,
                      ),
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            colors: [Colors.black54, Colors.transparent],
                            begin: Alignment.bottomCenter,
                            end: Alignment.topCenter,
                          ),
                        ),
                      ),
                      Positioned(
                        left: 8,
                        bottom: 8,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              pick['title'],
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            Text(
                              pick['subtitle'],
                              style:
                                  TextStyle(color: Colors.white, fontSize: 14),
                            ),
                          ],
                        ),
                      ),
                      Positioned(
                        bottom: 8,
                        right: 8,
                        child: Row(
                          children: [
                            Icon(Icons.favorite, color: Colors.white, size: 16),
                            SizedBox(width: 4),
                            Text(
                              pick['likes'].toString(),
                              style: TextStyle(color: Colors.white),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              );
            }).toList(),
          ),
        ],
      ),
    );
  }
}
