variable "tournament_db_user" {
  description = "Database username"
  type        = string
  sensitive   = true
}

variable "tournament_db_password" {
  description = "Database password"
  type        = string
  sensitive   = true
}

variable "db_name" {
  description = "Database name"
  type        = string
  default     = "tournaments"
}

variable "build_configuration" {
  description = "Build configuration for the application"
  type        = string
  default     = "Release"
}

variable "app_port_http" {
  description = "HTTP port for the application"
  type        = number
  default     = 8080
}

variable "app_port_metrics" {
  description = "Metrics port for the application"
  type        = number
  default     = 8081
}

variable "app_replicas" {
  description = "Number of application container replicas"
  type        = number
  default     = 1
}

variable "app_image_tag" {
  description = "Tag for app docker image"
  type        = string
  default     = "master"
}
