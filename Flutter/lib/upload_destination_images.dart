import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'dart:io';
import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:path/path.dart' as p;
import 'package:tajawul/custom_drawer.dart';

class UploadDestinationImages extends StatefulWidget {
  final String destinationId;

  const UploadDestinationImages({
    Key? key,
    required this.destinationId,
  }) : super(key: key);

  @override
  _UploadDestinationImagesState createState() =>
      _UploadDestinationImagesState();
}

class _UploadDestinationImagesState extends State<UploadDestinationImages> {
  File? _coverImage;
  List<File> _destinationImages = [];
  bool _isLoading = false;
  double _uploadProgress = 0;
  final Dio _dio = Dio();

  Future<void> _pickCoverImage() async {
    try {
      final picked = await ImagePicker().pickImage(
        source: ImageSource.gallery,
        imageQuality: 80,
        maxWidth: 1024,
        maxHeight: 1024,
      );

      if (picked != null) {
        final file = File(picked.path);
        final size = await file.length();

        if (size > 2 * 1024 * 1024) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('File size exceeds 2MB limit'),
                backgroundColor: Colors.red,
              ),
            );
          }
          return;
        }

        setState(() => _coverImage = file);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Error selecting image: ${e.toString()}'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _pickDestinationImages() async {
    try {
      final picked = await ImagePicker().pickMultiImage(
        imageQuality: 80,
        maxWidth: 1024,
        maxHeight: 1024,
      );

      if (picked != null) {
        if (picked.length > 5) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Maximum 5 images allowed'),
                backgroundColor: Colors.red,
              ),
            );
          }
          return;
        }

        for (var image in picked) {
          final file = File(image.path);
          final size = await file.length();
          if (size > 2 * 1024 * 1024) {
            if (mounted) {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('One or more files exceed 2MB limit'),
                  backgroundColor: Colors.red,
                ),
              );
            }
            return;
          }
        }

        setState(() {
          _destinationImages = picked.map((e) => File(e.path)).toList();
        });
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Error selecting images: ${e.toString()}'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _submitImages() async {
    if (_coverImage == null) {
      _showError('Cover image is required');
      return;
    }

    final token = await _getAuthToken();
    if (token == null) return;

    setState(() {
      _isLoading = true;
      _uploadProgress = 0;
    });

    try {
      // Upload cover image with 'ProfileImage' field name
      final coverResponse = await _dio.put(
        'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Destination/${widget.destinationId}/coverImage',
        data: FormData.fromMap({
          'ProfileImage': await MultipartFile.fromFile(
            _coverImage!.path,
            filename: p.basename(_coverImage!.path),
          ),
        }),
        options: Options(
          headers: {
            'Authorization': 'Bearer $token',
            'accept': '*/*',
            'Content-Type': 'multipart/form-data',
          },
          validateStatus: (status) => status! < 500,
        ),
        onSendProgress: (sent, total) {
          if (mounted) {
            setState(() {
              _uploadProgress = sent / total;
            });
          }
        },
      );

      if (coverResponse.statusCode != 200) {
        throw Exception('Failed to upload cover image: ${coverResponse.data}');
      }

      // Upload multiple images with 'Images' field name if any
      if (_destinationImages.isNotEmpty) {
        final formData = FormData();
        for (var image in _destinationImages) {
          formData.files.add(
            MapEntry(
              'Images',
              await MultipartFile.fromFile(
                image.path,
                filename: p.basename(image.path),
              ),
            ),
          );
        }

        final imagesResponse = await _dio.put(
          'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/Destination/${widget.destinationId}/images',
          data: formData,
          options: Options(
            headers: {
              'Authorization': 'Bearer $token',
              'accept': '*/*',
              'Content-Type': 'multipart/form-data',
            },
            validateStatus: (status) => status! < 500,
          ),
          onSendProgress: (sent, total) {
            if (mounted) {
              setState(() {
                _uploadProgress = sent / total;
              });
            }
          },
        );

        if (imagesResponse.statusCode != 200) {
          throw Exception('Failed to upload images: ${imagesResponse.data}');
        }
      }

      _showSuccess('Images uploaded successfully!');
      if (mounted) Navigator.pop(context, true);
    } catch (e) {
      _showError('Error uploading images: ${e.toString()}');
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _uploadProgress = 0;
        });
      }
    }
  }

  Future<String?> _getAuthToken() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('auth_token') ?? '';

    if (token.isEmpty && mounted) {
      _showError("Authentication required. Please login.");
    }

    return token.isNotEmpty ? token : null;
  }

  void _showError(String message) {
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(message),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 3),
        ),
      );
    }
  }

  void _showSuccess(String message) {
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(message),
          backgroundColor: Colors.green,
          duration: const Duration(seconds: 3),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFFAF7F2),
      drawer: const CustomDrawer(),
      appBar: AppBar(
        title: const Text("Images for Your Destination"),
        backgroundColor: Colors.brown,
      ),
      body: Stack(
        children: [
          Padding(
            padding: const EdgeInsets.all(20.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text("Accepted formats: JPG, JPEG, PNG, WEBP",
                    style: TextStyle(color: Colors.teal)),
                const Text("Maximum file size: 2MB",
                    style: TextStyle(color: Colors.teal)),
                const SizedBox(height: 20),

                // Cover Image Section
                const Text("Cover Image *",
                    style:
                        TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                const Text("Upload a cover image for your destination."),
                const SizedBox(height: 10),
                ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.black,
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)),
                  ),
                  onPressed: _isLoading ? null : _pickCoverImage,
                  icon: const Icon(Icons.image, color: Colors.white),
                  label: const Text("Upload Cover",
                      style: TextStyle(color: Colors.white)),
                ),
                if (_coverImage != null)
                  Padding(
                    padding: const EdgeInsets.only(top: 10),
                    child: Stack(
                      children: [
                        Image.file(_coverImage!,
                            height: 100, width: 100, fit: BoxFit.cover),
                        Positioned(
                          top: 5,
                          right: 5,
                          child: IconButton(
                            icon: const Icon(Icons.close, color: Colors.red),
                            onPressed: _isLoading
                                ? null
                                : () => setState(() => _coverImage = null),
                          ),
                        ),
                      ],
                    ),
                  ),

                const SizedBox(height: 30),

                // Destination Images Section
                const Text("Destination Images",
                    style:
                        TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                const Text("You can upload up to 5 additional images."),
                const SizedBox(height: 10),
                ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.black,
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)),
                  ),
                  onPressed: _isLoading ? null : _pickDestinationImages,
                  icon: const Icon(Icons.image, color: Colors.white),
                  label: const Text("Upload Images",
                      style: TextStyle(color: Colors.white)),
                ),
                const SizedBox(height: 10),
                Wrap(
                  spacing: 10,
                  children: _destinationImages
                      .map((img) => Stack(
                            children: [
                              Image.file(
                                img,
                                height: 60,
                                width: 60,
                                fit: BoxFit.cover,
                              ),
                              Positioned(
                                top: 0,
                                right: 0,
                                child: IconButton(
                                  icon: const Icon(Icons.close,
                                      size: 15, color: Colors.red),
                                  onPressed: _isLoading
                                      ? null
                                      : () => setState(
                                          () => _destinationImages.remove(img)),
                                ),
                              ),
                            ],
                          ))
                      .toList(),
                ),

                if (_uploadProgress > 0)
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 20),
                    child: LinearProgressIndicator(
                      value: _uploadProgress,
                      backgroundColor: Colors.grey[200],
                      color: Colors.brown,
                    ),
                  ),

                const Spacer(),

                Center(
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.brown,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10)),
                      minimumSize: const Size(double.infinity, 50),
                    ),
                    onPressed: _isLoading ? null : _submitImages,
                    child: _isLoading
                        ? const CircularProgressIndicator(color: Colors.white)
                        : const Text("Submit Images",
                            style: TextStyle(fontSize: 16)),
                  ),
                ),
              ],
            ),
          ),
          if (_isLoading && _uploadProgress == 0)
            Container(
              color: Colors.black.withOpacity(0.3),
              child: const Center(
                child: CircularProgressIndicator(
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.brown),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
