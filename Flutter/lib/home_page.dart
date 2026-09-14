import 'package:flutter/material.dart';
import 'package:tajawul/custom_search_bar.dart';
import 'package:tajawul/nav_bar.dart';
import 'package:tajawul/popular_trips_section.dart';
import 'package:tajawul/top_picks.dart';
import 'package:tajawul/trip_ai_section.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'custom_drawer.dart';

class TajawalHomePage extends StatefulWidget {
  @override
  _TajawalHomePageState createState() => _TajawalHomePageState();
}

class _TajawalHomePageState extends State<TajawalHomePage> {
  @override
  void initState() {
    super.initState();
    _fetchUserProfile();
  }

  Future<void> _fetchUserProfile() async {
    // First upload the image
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('auth_token') ?? '';

    if (token != null && token.isNotEmpty) {
      // Fetch user profile after login
      final profileResponse = await http.get(
        Uri.parse(
            'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/profile'),
        headers: {
          'accept': '*/*',
          'Authorization': 'Bearer ' + token,
        },
      );
      _showSnackBar(profileResponse.statusCode.toString());

      if (profileResponse.statusCode == 500) {
        Navigator.pushReplacementNamed(context, '/CompleteProfilePage');
      } else if (profileResponse.statusCode == 200) {
        // Parse the response body first
        final responseData = jsonDecode(profileResponse.body);
        if (responseData['firstName'] == null ||
            responseData['firstName'] == "") {
          Navigator.pushReplacementNamed(context, '/CompleteProfilePage');
        }
      }
    } else {
      _showSnackBar('user not login');
    }
  }

  void _showSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return Scaffold(
      drawer: const CustomDrawer(),
      body: Stack(
        children: [
          Container(color: Color(0xff141111)), // Set background to white
          SingleChildScrollView(
            child: Column(
              children: [
                Stack(
                  children: [
                    Container(
                      height: screenHeight *
                          .5, // Set a fixed height or use constraints
                      decoration: BoxDecoration(
                        image: DecorationImage(
                          image: AssetImage("assets/image.png"),
                          fit: BoxFit.cover,
                        ),
                      ),
                    ),
                    Container(
                      height: screenHeight *
                          .5, // Same height as the image container
                      color: Colors.black.withOpacity(0.5), // Dark overlay
                    ),
                    Positioned(
                      child: Column(
                        children: [
                          const NavBar(),
                          SizedBox(height: screenHeight * 0.03),
                          CustomSearchBar(),
                          SizedBox(height: screenHeight * 0.08),
                        ],
                      ),
                    ),
                  ],
                ),
                Container(
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.only(
                      topLeft: Radius.circular(30),
                      topRight: Radius.circular(30),
                    ),
                  ),
                  child: Column(
                    children: [
                      PopularTripsSection(),
                      SizedBox(height: screenHeight * 0.02),
                      TripAISection(),
                      // AdditionalContent(),
                      TopPicks()
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
