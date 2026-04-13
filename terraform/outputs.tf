output "app_ip" {
    value = docker_container.app[0].network_data[0].ip_address
}

output "app_container_name" {
    value = [for c in docker_container.app : c.name]
}

output "db_ip" {
    value = docker_container.db.network_data[0].ip_address
}

output "db_container_name" {
    value = docker_container.db.name
}