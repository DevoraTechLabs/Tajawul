import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:tajawul/add_destination.dart';
import 'package:tajawul/copleatprofile_screen.dart';
import 'package:tajawul/distnation.dart';
import 'package:tajawul/email_screen.dart';
import 'package:tajawul/explore.dart';
import 'package:tajawul/explore_trips_page.dart';
import 'package:tajawul/home_page.dart';
import 'package:tajawul/upload_destination_images.dart';
import 'signup.dart';
import 'forgetpassword.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'complete_profile_page.dart';
import 'models/destination.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: TajawalHomePage(),
      routes: {
        '/home': (context) => TajawalHomePage(),
        '/email': (context) => VerifyEmailScreen(),
        '/profilecom': (context) => VerifyProfileScreen(),
        '/addDestnation': (context) => AddDestinationPage(),
        '/explore': (context) => ExploreScreen(),
        '/login': (context) => const LoginScreen(),
        '/signup': (context) => SignUpScreen(),
        '/forgetpassword': (context) => ForgotPasswordScreen(),
        '/CompleteProfilePage': (context) => CompleteProfilePage(),
        '/trips': (context) => ExploreTripsPage(),
      },
      onGenerateRoute: (settings) {
        switch (settings.name) {
          case '/destination':
            final args = settings.arguments;
            if (args is Destination) {
              return MaterialPageRoute(
                builder: (context) => DestinationScreen(destination: args),
              );
            }
            return MaterialPageRoute(
              builder: (context) => Scaffold(
                body: Center(child: Text('No destination provided!')),
              ),
            );

          case '/uplodeDestinationImages':
            final destinationId = settings.arguments as String?;
            if (destinationId == null || destinationId.isEmpty) {
              return MaterialPageRoute(
                builder: (context) => Scaffold(
                  body: Center(child: Text('No destination ID provided!')),
                ),
              );
            }
            return MaterialPageRoute(
              builder: (context) =>
                  UploadDestinationImages(destinationId: destinationId),
            );

          default:
            return null;
        }
      },
    );
  }
}

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  bool isPasswordVisible = false;
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();
  bool isLoading = false;

  Future<void> _login() async {
    if (emailController.text.isEmpty || passwordController.text.isEmpty) {
      _showSnackBar('Please enter email and password');
      return;
    }

    setState(() => isLoading = true);
    try {
      final url = Uri.parse(
          'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Auth/signin');

      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': emailController.text.trim(),
          'password': passwordController.text.trim(),
        }),
      );

      if (response.statusCode == 201) {
        final data = jsonDecode(response.body);
        final token = data['token'];

        if (token != null) {
          await _saveToken(token);
          _showSnackBar('Login successful!');

          final profileResponse = await http.get(
            Uri.parse(
                'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/profile'),
            headers: {
              'accept': '*/*',
              'Authorization': 'Bearer $token',
            },
          );

          if (profileResponse.statusCode == 500) {
            Navigator.pushReplacementNamed(context, '/CompleteProfilePage');
          } else {
            Navigator.pushReplacementNamed(context, '/home');
          }
        } else {
          _showSnackBar('Token not received');
        }
      } else {
        final errorMessage =
            jsonDecode(response.body)['message'] ?? 'Login failed';
        _showSnackBar(errorMessage);
      }
    } catch (e) {
      _showSnackBar('Error: ${e.toString()}');
    } finally {
      setState(() => isLoading = false);
    }
  }

  Future<void> _saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('auth_token', token);
  }

  Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('auth_token');
  }

  void _showSnackBar(String message) {
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text(message)));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFFAF3E8),
      body: GestureDetector(
        onTap: () => FocusScope.of(context).unfocus(),
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const SizedBox(height: 50),
              IconButton(
                icon: const Icon(Icons.arrow_back, color: Colors.black),
                onPressed: () => Navigator.pop(context),
              ),
              Center(
                child: Column(
                  children: [
                    const SizedBox(height: 20),
                    SvgPicture.asset('assets/tajawul_logo.svg', height: 60),
                    const SizedBox(height: 16),
                    const Text('Welcome Back',
                        style: TextStyle(
                            fontSize: 28, fontWeight: FontWeight.w600)),
                    const SizedBox(height: 32),
                    ElevatedButton.icon(
                      onPressed: () {},
                      icon: const FaIcon(FontAwesomeIcons.google,
                          color: Colors.black),
                      label: const Text('Continue with Google',
                          style: TextStyle(color: Colors.black)),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFFF3DDBB),
                        padding: const EdgeInsets.symmetric(
                            vertical: 14, horizontal: 80),
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12)),
                      ),
                    ),
                    const SizedBox(height: 20),
                    _buildDivider('Or using Email'),
                    _buildTextField(
                        'Email Address or Username', emailController, false),
                    _buildTextField('Password', passwordController, true),
                    Align(
                      alignment: Alignment.centerLeft,
                      child: TextButton(
                        onPressed: () =>
                            Navigator.pushNamed(context, '/forgetpassword'),
                        child: const Text('Forgot Password?',
                            style: TextStyle(color: Color(0xFF567189))),
                      ),
                    ),
                    const SizedBox(height: 20),
                    ElevatedButton(
                      onPressed: isLoading ? null : _login,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF1D2D24),
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(
                            vertical: 14, horizontal: 100),
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12)),
                      ),
                      child: isLoading
                          ? const CircularProgressIndicator(color: Colors.white)
                          : const Text('Log In',
                              style: TextStyle(fontSize: 18)),
                    ),
                    const SizedBox(height: 20),
                    _buildDivider('Not a Member?'),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        GestureDetector(
                          onTap: () => Navigator.pushNamed(context, '/signup'),
                          child: const Text('Register',
                              style: TextStyle(
                                  color: Color(0xFF567189),
                                  fontWeight: FontWeight.bold)),
                        ),
                        const Text(' to unlock the best of Tajawul',
                            style: TextStyle(color: Colors.grey)),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDivider(String text) {
    return Row(
      children: [
        const Expanded(child: Divider(thickness: 1, color: Colors.grey)),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8),
          child: Text(text, style: const TextStyle(color: Colors.grey)),
        ),
        const Expanded(child: Divider(thickness: 1, color: Colors.grey)),
      ],
    );
  }

  Widget _buildTextField(
      String label, TextEditingController controller, bool isPassword) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label,
            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
        const SizedBox(height: 8),
        TextField(
          controller: controller,
          obscureText: isPassword ? !isPasswordVisible : false,
          decoration: InputDecoration(
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
            suffixIcon: isPassword
                ? IconButton(
                    icon: Icon(isPasswordVisible
                        ? Icons.visibility
                        : Icons.visibility_off),
                    onPressed: () =>
                        setState(() => isPasswordVisible = !isPasswordVisible),
                  )
                : null,
          ),
        ),
        const SizedBox(height: 16),
      ],
    );
  }
}
