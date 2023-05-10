# Impact Innovator API

### Architecture of the API

![Diagrama sin título](https://user-images.githubusercontent.com/57040777/236698243-ac45504c-aeef-4bbf-8ec5-e6ed57255057.jpg)

1. In the first step the API receives an audio file from the user.
2. In the steps 2 and 3 we are requesting credentials to a safely access to the aws services.
3. After that the .NET API (hosted in Elastic Beanstalk) saves this file in a s3 bucket.
4. Once the file is saved, we create a transcription job in the AWS Transcribe service in the region of Paris.
5. When the transcription job finish we can get the transcription file from the same s3 bucket where the audio file has been saved.
6. Finally, we return this file to the user, and fill the forms.
