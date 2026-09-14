import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:tajawul/trip.dart';
import 'package:tajawul/trip_cardex.dart';
import 'custom_drawer.dart';

class ExploreTripsPage extends StatefulWidget {
  const ExploreTripsPage({super.key});

  @override
  State<ExploreTripsPage> createState() => _ExploreTripsPageState();
}

class _ExploreTripsPageState extends State<ExploreTripsPage> {
  List<Trip> trips = [];
  bool isLoading = true;
  String errorMessage = '';

  @override
  void initState() {
    super.initState();
    fetchTrips();
  }

  Future<void> fetchTrips() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('auth_token') ?? '';

    if (token.isEmpty) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
              content: Text("Authentication required. Please login.")),
        );
        setState(() {
          isLoading = false;
          errorMessage = 'login required';
        });
      }
      return;
    }

    const String apiUrl =
        'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Trip';

    try {
      final response = await http.get(
        Uri.parse(apiUrl),
        headers: {
          'accept': '*/*',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        final List<dynamic> tripList = data['trips'];

        setState(() {
          trips = tripList.map((tripJson) => Trip.fromJson(tripJson)).toList();
          isLoading = false;
          errorMessage = '';
        });
      } else {
        setState(() {
          isLoading = false;
          errorMessage = 'Failed to load trips: ${response.statusCode}';
        });

        if (response.statusCode == 401 && mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
                content: Text("Session expired. Please login again.")),
          );
        }
      }
    } catch (e) {
      setState(() {
        isLoading = false;
        errorMessage = 'Error fetching trips: ${e.toString()}';
      });

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Network error: ${e.toString()}")),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const CustomDrawer(),
      appBar: AppBar(
        title: const Text('Explore Trips'),
        backgroundColor: const Color(0xff825b4e),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              setState(() {
                isLoading = true;
                errorMessage = '';
              });
              fetchTrips();
            },
          ),
        ],
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : errorMessage.isNotEmpty
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        errorMessage,
                        style: Theme.of(context).textTheme.bodyLarge,
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 20),
                      ElevatedButton(
                        onPressed: fetchTrips,
                        child: const Text('Retry'),
                      ),
                    ],
                  ),
                )
              : trips.isEmpty
                  ? const Center(
                      child: Text(
                        'No trips available',
                        style: TextStyle(fontSize: 18),
                      ),
                    )
                  : GridView.builder(
                      padding: const EdgeInsets.all(8),
                      itemCount: trips.length,
                      gridDelegate:
                          const SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: 2,
                        childAspectRatio: 0.75,
                        crossAxisSpacing: 8,
                        mainAxisSpacing: 8,
                      ),
                      itemBuilder: (context, index) {
                        return TripCard(trip: trips[index]);
                      },
                    ),
    );
  }
}
