# S3 Parallel Put

Upload folder files in parallel

Usage example:

```
s3-parallel-put.exe -t 10 -f "C:\Users\AlexandrM\Desktop\test" -k "********************" -s "****************************************" -z "eu-central-1" -b "rua-cv-photos" -a "public-read" -p "*.jpg" -r
```

Available options:

```
  -t, --threads      (Default: 10) Number of threads to run

  -f, --folder       Required. Folder with files to upload

  -k, --key          Required. AWS Access Key ID

  -s, --secret       Required. AWS Secret Key

  -z, --zone         Required. AWS Region e.g. eu-central-1, us-west-1

  -b, --bucket       Required. Bucket name to upload files into

  -a, --access       (Default: public-read) S3 canned ACL e.g. NoACL, private,
                     public-read

  -p, --pattern      (Default: *.*) Search pattern, e.g. *.jpg

  -r, --recursive    (Default: false) Recursively iterate
```