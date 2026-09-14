import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:tajawul/custom_drawer.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:tajawul/upload_destination_images.dart';

class AddDestinationPage extends StatelessWidget {
  const AddDestinationPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const CustomDrawer(),
      appBar: AppBar(
        title: const Text("Add Destination"),
        backgroundColor: Colors.brown,
        foregroundColor: Colors.white,
        elevation: 2,
      ),
      backgroundColor: const Color(0xFFFCF7F0),
      body: const SingleChildScrollView(
        child: Padding(
          padding: EdgeInsets.all(20.0),
          child: DestinationForm(),
        ),
      ),
    );
  }
}

class DestinationForm extends StatefulWidget {
  const DestinationForm({super.key});

  @override
  State<DestinationForm> createState() => _DestinationFormState();
}

class _DestinationFormState extends State<DestinationForm> {
  bool isOpen24Hours = false;
  String? selectedCity;
  String? selectedType;
  String? selectedCountry;
  String? selectedPriceRange;

  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _descController = TextEditingController();
  final TextEditingController _establishedController = TextEditingController();
  final TextEditingController _addressController = TextEditingController();
  final TextEditingController _longitudeController = TextEditingController();
  final TextEditingController _latitudeController = TextEditingController();

  final List<Map<String, String>> socialMediaLinks = [];
  final TextEditingController _platformController = TextEditingController();
  final TextEditingController _urlController = TextEditingController();

  final _formKey = GlobalKey<FormState>();
  bool _isLoading = false;

  Future<void> _submitForm() async {
    if (_formKey.currentState!.validate()) {
      setState(() {
        _isLoading = true;
      });

      try {
        final prefs = await SharedPreferences.getInstance();
        final token = prefs.getString('auth_token') ?? '';

        if (token.isEmpty) {
          if (context.mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                  content: Text("Authentication required. Please login.")),
            );
          }
          return;
        }

        final body = {
          "name": _nameController.text,
          "description": _descController.text,
          "type": selectedType,
          "priceRange": selectedPriceRange?.toLowerCase(),
          "country": selectedCountry?.toLowerCase(),
          "city": selectedCity?.toLowerCase(),
          "locations": [
            {
              "longitude": double.tryParse(_longitudeController.text) ?? 0,
              "latitude": double.tryParse(_latitudeController.text) ?? 0,
              "address": _addressController.text
            }
          ],
          "isOpen24Hours": isOpen24Hours,
          "establishedAt": _establishedController.text,
          "socialMediaLinks": socialMediaLinks,
        };

        final url = Uri.parse(
            "https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Destination");
        final response = await http.post(
          url,
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer $token",
          },
          body: json.encode(body),
        );

        if (response.statusCode == 200 || response.statusCode == 201) {
          final responseData = json.decode(response.body);
          _showResponseBottomSheet(responseData);
          final createdDestinationId =
              responseData['id'] ?? responseData['destinationId'];

          if (context.mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                  content: Text("Destination Created Successfully!")),
            );

            // Navigate to image upload screen
            Navigator.push(
              context,
              MaterialPageRoute(
                builder: (context) => UploadDestinationImages(
                  destinationId: createdDestinationId.toString(),
                ),
              ),
            );
          }
        } else {
          if (context.mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                  content:
                      Text("Error: ${response.statusCode} - ${response.body}")),
            );
          }
        }
      } catch (e) {
        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text("Error: ${e.toString()}")),
          );
        }
      } finally {
        if (context.mounted) {
          setState(() {
            _isLoading = false;
          });
        }
      }
    }
  }

  void _showResponseBottomSheet(Map<String, dynamic> responseData) {
    showModalBottomSheet(
      context: context,
      builder: (context) {
        return Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'API Response',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              Expanded(
                child: SingleChildScrollView(
                  child: Text(
                    responseData.toString(),
                    style: const TextStyle(fontFamily: 'monospace'),
                  ),
                ),
              ),
              const SizedBox(height: 10),
              ElevatedButton(
                onPressed: () => Navigator.pop(context),
                child: const Text('Close'),
              ),
            ],
          ),
        );
      },
    );
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descController.dispose();
    _establishedController.dispose();
    _addressController.dispose();
    _longitudeController.dispose();
    _latitudeController.dispose();
    _platformController.dispose();
    _urlController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const sectionTextStyle = TextStyle(
      fontWeight: FontWeight.w600,
      fontSize: 13,
      letterSpacing: 1.2,
      color: Colors.grey,
    );

    return Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SizedBox(height: 10),
          const Text("GENERAL INFO", style: sectionTextStyle),
          const Divider(),
          _buildTextField(
              controller: _nameController, label: 'Destination Name'),
          Row(
            children: [
              Expanded(
                child: _buildDropdown(
                  label: "Destination Type",
                  items: const ["Museum", "Park", "Hotel", "Co-Working Space"],
                  onChanged: (val) => selectedType = val,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _buildDropdown(
                  label: "Country",
                  items: const ["USA", "Egypt", "France"],
                  onChanged: (val) => selectedCountry = val,
                ),
              ),
            ],
          ),
          _buildTextField(
            controller: _descController,
            label: 'Description',
            maxLines: 3,
          ),
          _buildTextField(
            controller: _establishedController,
            label: 'Established At',
            hint: 'yyyy-mm-dd',
          ),
          const SizedBox(height: 24),
          const Text("LOCATION DETAILS", style: sectionTextStyle),
          const Divider(),
          _buildDropdown(
            label: "City",
            items: const ["Cairo", "Alexandria", "Paris", "NYC"],
            onChanged: (val) => selectedCity = val,
          ),
          _buildTextField(
            controller: _addressController,
            label: "Address",
          ),
          Row(
            children: [
              Expanded(
                child: _buildTextField(
                    controller: _longitudeController, label: "Longitude"),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _buildTextField(
                    controller: _latitudeController, label: "Latitude"),
              ),
            ],
          ),
          const SizedBox(height: 24),
          const Text("OPERATING HOURS & PRICING", style: sectionTextStyle),
          const Divider(),
          CheckboxListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text("Open 24 Hours"),
            value: isOpen24Hours,
            onChanged: (val) {
              setState(() {
                isOpen24Hours = val!;
              });
            },
          ),
          if (!isOpen24Hours)
            Row(
              children: [
                Expanded(
                  child: _buildTextField(label: "Open Time", hint: "--:-- --"),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: _buildTextField(label: "Close Time", hint: "--:-- --"),
                ),
              ],
            ),
          _buildDropdown(
            label: "Price Range",
            items: const ["Free", "Low", "Medium", "High"],
            onChanged: (val) => selectedPriceRange = val,
          ),
          const SizedBox(height: 24),
          const Text("CONTACT & SOCIAL MEDIA", style: sectionTextStyle),
          const Divider(),
          _buildTextField(
            controller: _platformController,
            label: "Social Media Platform",
          ),
          const SizedBox(height: 8),
          _buildTextField(
            controller: _urlController,
            label: "Social Media URL",
          ),
          TextButton.icon(
            onPressed: () {
              if (_platformController.text.isNotEmpty &&
                  _urlController.text.isNotEmpty) {
                setState(() {
                  socialMediaLinks.add({
                    "platform": _platformController.text,
                    "url": _urlController.text,
                  });
                  _platformController.clear();
                  _urlController.clear();
                });
              }
            },
            icon: const Icon(Icons.add),
            label: const Text("Add Social Link"),
          ),
          Wrap(
            spacing: 8,
            children: socialMediaLinks
                .map((e) => Chip(label: Text(e['platform']!)))
                .toList(),
          ),
          const SizedBox(height: 25),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: _isLoading ? null : _submitForm,
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.brown,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(vertical: 16),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: _isLoading
                  ? const CircularProgressIndicator(color: Colors.white)
                  : const Text(
                      "Create Destination",
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTextField({
    required String label,
    String? hint,
    int maxLines = 1,
    TextEditingController? controller,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: TextFormField(
        controller: controller,
        maxLines: maxLines,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          contentPadding:
              const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
        validator: (val) => val == null || val.isEmpty ? 'Required' : null,
      ),
    );
  }

  Widget _buildDropdown({
    required String label,
    required List<String> items,
    required Function(String?) onChanged,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: DropdownButtonFormField<String>(
        decoration: InputDecoration(
          labelText: label,
          contentPadding:
              const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
        items: items
            .map((e) => DropdownMenuItem(value: e, child: Text(e)))
            .toList(),
        onChanged: onChanged,
        validator: (val) => val == null ? 'Required' : null,
      ),
    );
  }
}
