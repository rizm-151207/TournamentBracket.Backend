
locals {
  network_name = "tournament-bracket-network"
  db_container_name = "tournament-bracket-db"
  app_container_name = "tournament-bracket-api"
}

resource "docker_network" "tournament_net" {
  name   = local.network_name
  driver = "bridge"
}

resource "docker_image" "app_image"{
  name = "ghcr.io/rizm-151207/tournamentbracket.backend:${var.app_image_tag}"
}

resource "docker_volume" "postgres_data" {
  name = "tournament-bracket-postgres-data"
}

resource "docker_container" "db" {
  image     = "postgres:16-alpine"
  name      = local.db_container_name
  restart   = "unless-stopped"

  ports {
    internal = 5432
    external = 6432
  }

  env = [
    "POSTGRES_DB=${var.db_name}",
    "POSTGRES_USER=${var.tournament_db_user}",
    "POSTGRES_PASSWORD=${var.tournament_db_password}"
  ]

  networks_advanced {
    name = docker_network.tournament_net.name
  }

  mounts {
    type     = "volume"
    source   = docker_volume.postgres_data.name
    target   = "/var/lib/postgresql/data"
  }

  healthcheck {
    test         = ["CMD-SHELL", "pg_isready -U ${var.tournament_db_user} -d ${var.db_name}"]
    interval     = "10s"
    timeout      = "5s"
    retries      = 5
    start_period = "30s"
  }

  depends_on = [docker_volume.postgres_data, docker_network.tournament_net]


  must_run = true
}

resource "docker_container" "app" {
  count = var.app_replicas

  image = docker_image.app_image.name
  name    = var.app_replicas > 1 ? "${local.app_container_name}-${count.index + 1}" : local.app_container_name
  restart = "unless-stopped"

  ports {
    internal = var.app_port_http
    external = var.app_port_http
  }

  ports {
    internal = var.app_port_metrics
    external = var.app_port_metrics
  }

  env = [
    "DB_HOST=${local.db_container_name}",
    "TOURNAMENT_DB_USER=${var.tournament_db_user}",
    "TOURNAMENT_DB_PASSWORD=${var.tournament_db_password}",
    "ASPNETCORE_ENVIRONMENT=Production",
    "ASPNETCORE_URLS=http://+:${var.app_port_http}"
  ]

  networks_advanced {
    name = docker_network.tournament_net.name
  }

  depends_on = [docker_container.db, docker_image.app_image,  docker_network.tournament_net]

  must_run = true
}
