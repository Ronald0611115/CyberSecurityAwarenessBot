using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberSecurityAwarenessBot.Models;

namespace CyberSecurityAwarenessBot.Services
{
    
    /// Handles all MySQL database operations for tasks.
    /// This is the ONLY class in the project that talks to the database directly.
    
    public class DatabaseService
    {
        // Default XAMPP MySQL credentials — no password by default
        private readonly string _connectionString =
            "Server=localhost;Database=cybersecuritybot;Uid=root;Pwd=;";

        
        /// Tests whether the database connection is working.
        
        public bool TestConnection()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        
        /// Inserts a new task and returns its generated TaskId.
        
        public int InsertTask(TaskItem task)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string query = @"INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted)
                                  VALUES (@title, @description, @reminder, @completed);
                                  SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", task.Title);
                cmd.Parameters.AddWithValue("@description", task.Description);
                cmd.Parameters.AddWithValue("@reminder", (object)task.ReminderDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@completed", task.IsCompleted);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insert task failed: {ex.Message}");
                return -1;
            }
        }

        
        /// Retrieves all tasks from the database, newest first.
        
        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string query = @"SELECT TaskId, Title, Description, ReminderDate, IsCompleted, CreatedAt
                                  FROM tasks ORDER BY CreatedAt DESC;";

                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetInt32("TaskId"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? string.Empty : reader.GetString("Description"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate"))
                                        ? (DateTime?)null : reader.GetDateTime("ReminderDate"),
                        IsCompleted = reader.GetBoolean("IsCompleted"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get tasks failed: {ex.Message}");
            }

            return tasks;
        }

        
        /// Marks a task as completed in the database.
        
        public void MarkTaskCompleted(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string query = "UPDATE tasks SET IsCompleted = TRUE WHERE TaskId = @id;";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mark completed failed: {ex.Message}");
            }
        }

        
        /// Permanently deletes a task from the database.
        
        public void DeleteTask(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string query = "DELETE FROM tasks WHERE TaskId = @id;";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete task failed: {ex.Message}");
            }
        }
    }
}
